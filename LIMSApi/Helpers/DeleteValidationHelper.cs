using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Centralized, generic delete validation helper for all master entities.
    /// Auto-detects foreign key dependencies using EF Core metadata, traverses parent-child
    /// relationships to resolve friendly business entity names/titles, and evaluates custom hooks.
    /// </summary>
    public static class DeleteValidationHelper
    {
        // Candidate descriptor columns in order of priority
        private static readonly string[] PreferredDescriptorColumns = new[]
        {
            "DisplayTitle", "Name", "FullName", "Title", "SpecificationNo", "InwardNo",
            "ReportNo", "StandardName", "Standard", "ScopeName", "TestMethodStandard",
            "Code", "Grade", "AliasName", "Description"
        };

        /// <summary>
        /// Validates whether an entity can be safely deleted by checking all foreign key references,
        /// traversing parent relationships to resolve friendly names/codes, and evaluating custom rules.
        /// </summary>
        /// <typeparam name="T">Entity type being deleted</typeparam>
        /// <param name="context">LIMSContext instance</param>
        /// <param name="entityId">Primary key of the entity to delete</param>
        /// <param name="entityDisplayName">Friendly category name (e.g., "Parameter", "Customer", "Equipment")</param>
        /// <param name="entityItemName">Specific name/identifier of the entity (e.g., "Tensile Strength", "Tata Steel")</param>
        public static async Task ValidateDeleteAsync<T>(
            LIMSContext context,
            long entityId,
            string? entityDisplayName = null,
            string? entityItemName = null) where T : class
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            if (entityType == null) return;

            var displayName = entityDisplayName ?? GetFriendlyEntityName(typeof(T).Name);
            var itemName = entityItemName ?? await TryResolveEntityNameAsync<T>(context, entityId) ?? $"ID: {entityId}";

            var blockingDependencies = new List<string>();

            // 1. Discover and validate all referencing Foreign Keys
            var referencingFKs = GetReferencingForeignKeys(context, entityType);

            // Group FKs by declaring entity type to avoid duplicate category rows
            var fksByEntity = referencingFKs
                .GroupBy(fk => fk.DeclaringEntityType)
                .ToList();

            foreach (var group in fksByEntity)
            {
                var dependentEntityType = group.Key;
                var dependentClrType = dependentEntityType.ClrType;

                // Skip self-referencing hierarchy checks on same entity (handled by specific service logic if needed)
                if (dependentClrType == typeof(T)) continue;

                // Skip child option tables that belong exclusively to the parent entity being deleted
                if (IsOwnedChildCollection(typeof(T), dependentClrType)) continue;

                foreach (var fk in group)
                {
                    var fkPropertyName = fk.Properties.First().Name;
                    var dependencySummary = await ResolveDependencySummaryAsync(
                        context, dependentEntityType, fkPropertyName, entityId);

                    if (!string.IsNullOrWhiteSpace(dependencySummary))
                    {
                        blockingDependencies.Add(dependencySummary);
                    }
                }
            }

            // 2. Evaluate custom domain hooks (formulas, comma-separated configuration IDs, etc.)
            var customDependencies = await EvaluateCustomDependenciesAsync<T>(context, entityId);
            if (customDependencies.Any())
            {
                blockingDependencies.AddRange(customDependencies);
            }

            // 3. If any blocking dependencies exist, throw a descriptive, formatted exception
            if (blockingDependencies.Any())
            {
                var bulletList = string.Join("\n• ", blockingDependencies.Distinct());
                throw new InvalidOperationException(
                    $"Cannot delete {displayName} '{itemName}' (ID: {entityId}) because it is linked to the following:\n• {bulletList}\n\nPlease remove or unlink this {displayName.ToLowerInvariant()} from the above module(s) first before deleting.");
            }
        }

        /// <summary>
        /// Resolves a human-readable dependency summary (with entity counts and sample names/titles)
        /// for a specific dependent table referencing the target entity.
        /// </summary>
        private static async Task<string?> ResolveDependencySummaryAsync(
            LIMSContext context,
            IEntityType dependentEntityType,
            string fkPropertyName,
            long entityId)
        {
            var tableName = dependentEntityType.GetTableName();
            if (string.IsNullOrEmpty(tableName)) return null;

            var schema = dependentEntityType.GetSchema() ?? "dbo";
            var fkProperty = dependentEntityType.FindProperty(fkPropertyName);
            var columnName = fkProperty?.GetColumnName() ?? fkPropertyName;
            var hasIsActive = dependentEntityType.FindProperty("IsActive") != null;

            // 1. Try specialized parent-traversal query for known junction/line tables
            var specializedResult = await TryResolveSpecializedDependencyAsync(context, tableName, columnName, entityId);
            if (specializedResult != null)
            {
                return specializedResult;
            }

            // 2. Count total active dependent records (EF Core requires AS [Value] for scalar SqlQueryRaw<T>)
            var countSql = hasIsActive
                ? $"SELECT COUNT(*) AS [Value] FROM [{schema}].[{tableName}] WHERE [{columnName}] = @p0 AND [IsActive] = 1"
                : $"SELECT COUNT(*) AS [Value] FROM [{schema}].[{tableName}] WHERE [{columnName}] = @p0";

            int count;
            try
            {
                count = await context.Database.SqlQueryRaw<int>(countSql, entityId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteValidationHelper] Error counting dependencies in {tableName}.{columnName}: {ex.Message}");
                return null;
            }

            if (count <= 0) return null;

            var friendlyName = GetFriendlyEntityName(dependentEntityType.ClrType.Name);

            // 3. Attempt to fetch sample descriptor names from the dependent table directly
            var descColumn = GetDescriptorColumnName(dependentEntityType);
            if (!string.IsNullOrEmpty(descColumn))
            {
                var sampleSql = hasIsActive
                    ? $"SELECT DISTINCT TOP 5 CAST([{descColumn}] AS NVARCHAR(MAX)) AS [Value] FROM [{schema}].[{tableName}] WHERE [{columnName}] = @p0 AND [{descColumn}] IS NOT NULL AND [IsActive] = 1"
                    : $"SELECT DISTINCT TOP 5 CAST([{descColumn}] AS NVARCHAR(MAX)) AS [Value] FROM [{schema}].[{tableName}] WHERE [{columnName}] = @p0 AND [{descColumn}] IS NOT NULL";

                try
                {
                    var sampleNames = await context.Database
                        .SqlQueryRaw<string>(sampleSql, entityId)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToListAsync();

                    if (sampleNames.Any())
                    {
                        var moreText = count > sampleNames.Count ? $" (+{count - sampleNames.Count} more)" : "";
                        return $"{friendlyName} ({count}): {string.Join(", ", sampleNames)}{moreText}";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DeleteValidationHelper] Error fetching descriptors in {tableName}.{descColumn}: {ex.Message}");
                }
            }

            return $"{friendlyName}: {count} recorded record{(count > 1 ? "s" : "")}";
        }

        /// <summary>
        /// Specialized parent-traversal queries for complex junction/line tables in LIMS.
        /// Resolves child FKs directly to their parent master business titles/names.
        /// </summary>
        private static async Task<string?> TryResolveSpecializedDependencyAsync(
            LIMSContext context,
            string tableName,
            string columnName,
            long entityId)
        {
            try
            {
                // Material Specification Lines -> Specification Grade -> Specification Header
                if (tableName.Equals("SpecificationLines", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 
                            (COALESCE(h.DisplayTitle, h.SpecificationNo, h.AliasName, 'Specification') + 
                            CASE WHEN g.Grade IS NOT NULL AND g.Grade <> '' THEN ' (Grade: ' + g.Grade + ')' ELSE '' END) AS [Value]
                        FROM dbo.SpecificationLines sl
                        LEFT JOIN dbo.SpecificationGrades g ON sl.SpecificationGradeID = g.ID
                        LEFT JOIN dbo.SpecificationHeaders h ON g.SpecificationHeaderID = h.ID
                        WHERE sl." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.SpecificationLines WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Material Specifications ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Specification Header Template Parameters -> Specification Header
                if (tableName.Equals("SpecificationHeaderParameters", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 
                            (COALESCE(h.DisplayTitle, h.SpecificationNo, h.AliasName, 'Specification') + ' (Template)') AS [Value]
                        FROM dbo.SpecificationHeaderParameters p
                        JOIN dbo.SpecificationHeaders h ON p.SpecificationHeaderID = h.ID
                        WHERE p." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.SpecificationHeaderParameters WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Material Specification Templates ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Laboratory Test Sub-Group Parameters -> Laboratory Test & Sub-Group
                if (tableName.Equals("LaboratoryTestSubGroupParameters", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 
                            (COALESCE(t.Name, 'Test') + ' -> ' + COALESCE(sg.Name, 'SubGroup')) AS [Value]
                        FROM dbo.LaboratoryTestSubGroupParameters p
                        JOIN dbo.LaboratoryTestSubGroups sg ON p.LaboratoryTestSubGroupID = sg.ID
                        JOIN dbo.LaboratoryTests t ON sg.LaboratoryTestID = t.ID
                        WHERE p." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.LaboratoryTestSubGroupParameters WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Laboratory Tests ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Laboratory Test Analysis Type Parameters -> Laboratory Test & Analysis Type
                if (tableName.Equals("LaboratoryTestAnalysisTypeParameters", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 
                            (COALESCE(t.Name, 'Test') + ' -> ' + COALESCE(at.Name, 'Analysis')) AS [Value]
                        FROM dbo.LaboratoryTestAnalysisTypeParameters p
                        JOIN dbo.LaboratoryTestAnalysisTypes at ON p.LaboratoryTestAnalysisTypeID = at.ID
                        JOIN dbo.LaboratoryTests t ON at.LaboratoryTestID = t.ID
                        WHERE p." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.LaboratoryTestAnalysisTypeParameters WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Laboratory Tests ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Lab Scope Specification Parameters -> Lab Scope & Test Method Spec
                if (tableName.Equals("LabScopeSpecificationParameters", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 
                            (COALESCE(ls.ScopeName, 'Lab Scope') + CASE WHEN tms.Name IS NOT NULL THEN ' (Method: ' + tms.Name + ')' ELSE '' END) AS [Value]
                        FROM dbo.LabScopeSpecificationParameters p
                        JOIN dbo.LabScopeSpecifications lss ON p.LabScopeSpecificationID = lss.ID
                        LEFT JOIN dbo.LabScopeMasters ls ON lss.LabScopeID = ls.ID
                        LEFT JOIN dbo.TestMethodSpecifications tms ON lss.TestMethodSpecificationID = tms.ID
                        WHERE p." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.LabScopeSpecificationParameters WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Lab Scope ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Test Method Specification Parameters -> Test Method Specification & Version
                if (tableName.Equals("TestMethodSpecificationParameters", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 
                            (COALESCE(tms.Name, 'Test Method') + CASE WHEN v.Version IS NOT NULL THEN ' (v' + v.Version + ')' ELSE '' END) AS [Value]
                        FROM dbo.TestMethodSpecificationParameters p
                        JOIN dbo.TestMethodSpecificationVersions v ON p.TestMethodSpecificationVersionID = v.ID
                        JOIN dbo.TestMethodSpecifications tms ON v.TestMethodSpecificationID = tms.ID
                        WHERE p." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.TestMethodSpecificationParameters WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Test Method Specifications ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Chemical Test Elements -> Chemical Tests
                if (tableName.Equals("ChemicalTestElements", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 COALESCE(ct.Name, 'Chemical Test') AS [Value]
                        FROM dbo.ChemicalTestElements e
                        JOIN dbo.ChemicalTests ct ON e.ChemicalTestID = ct.ID
                        WHERE e." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.ChemicalTestElements WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Chemical Tests ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Metal Classification Parameters -> Metal Classification Master
                if (tableName.Equals("MetalClassificationParameters", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 m.Name AS [Value]
                        FROM dbo.MetalClassificationParameters p
                        JOIN dbo.MetalClassificationMasters m ON p.MetalClassificationID = m.ID
                        WHERE p." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.MetalClassificationParameters WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Metal Classifications ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Sample Test Plans -> Sample Inwards
                if (tableName.Equals("SampleTestPlans", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 COALESCE(i.InwardNo, 'Sample Inward') AS [Value]
                        FROM dbo.SampleTestPlans p
                        JOIN dbo.SampleDetails sd ON p.SampleDetailID = sd.ID
                        JOIN dbo.SampleInwards i ON sd.SampleInwardID = i.ID
                        WHERE p." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.SampleTestPlans WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Sample Test Plans ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Sample Details -> Sample Inwards
                if (tableName.Equals("SampleDetails", StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
                        SELECT DISTINCT TOP 5 COALESCE(i.InwardNo, 'Sample Inward') AS [Value]
                        FROM dbo.SampleDetails sd
                        JOIN dbo.SampleInwards i ON sd.SampleInwardID = i.ID
                        WHERE sd." + columnName + " = @p0";

                    var names = await context.Database.SqlQueryRaw<string>(sql, entityId).ToListAsync();
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.SampleDetails WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        var more = count > names.Count ? $" (+{count - names.Count} more)" : "";
                        return $"Sample Inwards ({count}): {string.Join(", ", names)}{more}";
                    }
                }

                // Test Result Parameters
                if (tableName.Equals("TestResultParameters", StringComparison.OrdinalIgnoreCase))
                {
                    var count = await context.Database.SqlQueryRaw<int>(
                        "SELECT COUNT(*) AS [Value] FROM dbo.TestResultParameters WHERE " + columnName + " = @p0", entityId).FirstOrDefaultAsync();

                    if (count > 0)
                    {
                        return $"Test Results: {count} recorded test result(s)";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteValidationHelper] Specialized dependency resolution error for {tableName}.{columnName}: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Evaluates non-FK dependencies such as Formula references, JSON expressions,
        /// or comma-separated configuration IDs.
        /// </summary>
        private static async Task<List<string>> EvaluateCustomDependenciesAsync<T>(LIMSContext context, long entityId)
        {
            var results = new List<string>();

            // ParameterMaster custom rules
            if (typeof(T) == typeof(ParameterMaster))
            {
                // 1. Other Parameters referencing this Parameter in Formula ({P191})
                var paramPattern = $"{{P{entityId}}}";
                var formulaParams = await context.ParameterMasters
                    .AsNoTracking()
                    .Where(p => p.IsActive && p.ID != entityId && p.IsCalculated && p.Formula != null &&
                                (p.Formula.Contains(paramPattern) || p.Formula.Contains($"{{P{entityId},")))
                    .Select(p => p.Name)
                    .Distinct()
                    .ToListAsync();

                if (formulaParams.Any())
                {
                    var sample = formulaParams.Take(5).ToList();
                    var more = formulaParams.Count > 5 ? $" (+{formulaParams.Count - 5} more)" : "";
                    results.Add($"Calculated Parameter Formulas ({formulaParams.Count}): {string.Join(", ", sample)}{more}");
                }

                // 2. Invoice Case Configurations (SourceParameterIDs / OverrideParameterIDs)
                var idStr = entityId.ToString();
                var invoiceConfigs = await context.InvoiceCaseConfigurations
                    .AsNoTracking()
                    .Where(c => c.IsActive && (
                        (c.SourceParameterIDs != null && (c.SourceParameterIDs == idStr || c.SourceParameterIDs.StartsWith(idStr + ",") || c.SourceParameterIDs.EndsWith("," + idStr) || c.SourceParameterIDs.Contains("," + idStr + ","))) ||
                        (c.OverrideParameterIDs != null && (c.OverrideParameterIDs == idStr || c.OverrideParameterIDs.StartsWith(idStr + ",") || c.OverrideParameterIDs.EndsWith("," + idStr) || c.OverrideParameterIDs.Contains("," + idStr + ",")))
                    ))
                    .Select(c => c.Name)
                    .Distinct()
                    .ToListAsync();

                if (invoiceConfigs.Any())
                {
                    var sample = invoiceConfigs.Take(5).ToList();
                    var more = invoiceConfigs.Count > 5 ? $" (+{invoiceConfigs.Count - 5} more)" : "";
                    results.Add($"Invoice Case Configurations ({invoiceConfigs.Count}): {string.Join(", ", sample)}{more}");
                }
            }

            return results;
        }

        /// <summary>
        /// Resolves the name/title of the target entity dynamically if not explicitly provided.
        /// </summary>
        private static async Task<string?> TryResolveEntityNameAsync<T>(LIMSContext context, long entityId) where T : class
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            if (entityType == null) return null;

            var tableName = entityType.GetTableName();
            var schema = entityType.GetSchema() ?? "dbo";
            var pkProperty = entityType.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name ?? "ID";

            var descColumn = GetDescriptorColumnName(entityType);
            if (string.IsNullOrEmpty(descColumn)) return null;

            try
            {
                var sql = $"SELECT TOP 1 CAST([{descColumn}] AS NVARCHAR(MAX)) AS [Value] FROM [{schema}].[{tableName}] WHERE [{pkProperty}] = @p0";
                var result = await context.Database.SqlQueryRaw<string>(sql, entityId).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteValidationHelper] TryResolveEntityNameAsync error for {typeof(T).Name} ID {entityId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Finds all foreign keys from other entities that reference the given entity type.
        /// </summary>
        private static List<IForeignKey> GetReferencingForeignKeys(LIMSContext context, IEntityType entityType)
        {
            var referencingFKs = new List<IForeignKey>();

            foreach (var otherEntityType in context.Model.GetEntityTypes())
            {
                foreach (var fk in otherEntityType.GetForeignKeys())
                {
                    if (fk.PrincipalEntityType == entityType)
                    {
                        referencingFKs.Add(fk);
                    }
                }
            }

            return referencingFKs;
        }

        /// <summary>
        /// Checks whether the dependent entity is an owned child collection of the parent
        /// (e.g. ParameterDropdownOption for ParameterMaster) which should be soft-deleted with the parent.
        /// </summary>
        private static bool IsOwnedChildCollection(Type parentType, Type dependentType)
        {
            if (parentType == typeof(ParameterMaster) && dependentType == typeof(ParameterDropdownOption))
                return true;

            return false;
        }

        /// <summary>
        /// Finds the best descriptor column name for a given entity type.
        /// </summary>
        private static string? GetDescriptorColumnName(IEntityType entityType)
        {
            foreach (var col in PreferredDescriptorColumns)
            {
                var prop = entityType.FindProperty(col);
                if (prop != null)
                {
                    return prop.GetColumnName();
                }
            }

            return null;
        }

        /// <summary>
        /// Converts PascalCase entity names to friendly display names.
        /// e.g., "EmployeeMaster" → "Employee", "SampleInward" → "Sample Inward"
        /// </summary>
        public static string GetFriendlyEntityName(string entityName)
        {
            var name = entityName
                .Replace("Master", "")
                .Replace("master", "")
                .Replace("Nabl", "NABL ");

            var result = string.Concat(name.Select((c, i) =>
                i > 0 && char.IsUpper(c) && !char.IsUpper(name[i - 1]) ? " " + c : c.ToString()));

            return result.Trim();
        }
    }
}
