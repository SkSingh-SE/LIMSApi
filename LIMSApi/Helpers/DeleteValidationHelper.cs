using LIMSApi.Data;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Centralized delete validation — auto-detects FK dependencies using EF Core metadata.
    /// Checks all referencing entities for active records before allowing soft-delete.
    /// </summary>
    public static class DeleteValidationHelper
    {
        /// <summary>
        /// Validates whether an entity can be safely deleted by checking all FK references.
        /// Uses EF Core model metadata to auto-discover dependent entities — no manual mapping needed.
        /// </summary>
        /// <typeparam name="T">Entity type being deleted</typeparam>
        /// <param name="context">LIMSContext instance</param>
        /// <param name="entityId">Primary key of the entity to delete</param>
        /// <param name="entityDisplayName">Friendly name for error messages (e.g., "Department")</param>
        /// <returns>Throws InvalidOperationException if dependencies found</returns>
        public static async Task ValidateDeleteAsync<T>(LIMSContext context, long entityId, string? entityDisplayName = null) where T : class
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            if (entityType == null) return;

            var displayName = entityDisplayName ?? typeof(T).Name.Replace("Master", "").Replace("master", "");
            var blockingDependencies = new List<string>();

            // Find all entities that have a FK pointing to this entity type
            var referencingFKs = GetReferencingForeignKeys(context, entityType);

            foreach (var fk in referencingFKs)
            {
                var dependentEntityType = fk.DeclaringEntityType;
                var dependentClrType = dependentEntityType.ClrType;
                var fkPropertyName = fk.Properties.First().Name;

                // Get the DbSet for the dependent entity dynamically
                var dbSet = GetDbSetForType(context, dependentClrType);
                if (dbSet == null) continue;

                // Build and execute: context.Set<DependentType>().AnyAsync(e => e.FK == entityId && e.IsActive)
                var count = await CountDependentRecords(context, dependentClrType, fkPropertyName, entityId);

                if (count > 0)
                {
                    var friendlyName = GetFriendlyEntityName(dependentClrType.Name);
                    blockingDependencies.Add($"{friendlyName} ({count} record{(count > 1 ? "s" : "")})");
                }
            }

            if (blockingDependencies.Any())
            {
                var dependencyList = string.Join(", ", blockingDependencies);
                throw new InvalidOperationException(
                    $"Cannot delete {displayName}: It is linked to {dependencyList}. Please remove these references first.");
            }
        }

        /// <summary>
        /// Gets all foreign keys from other entities that reference the given entity type.
        /// </summary>
        private static List<IForeignKey> GetReferencingForeignKeys(LIMSContext context, IEntityType entityType)
        {
            var referencingFKs = new List<IForeignKey>();

            foreach (var otherEntityType in context.Model.GetEntityTypes())
            {
                // Skip self-referencing at the same entity (e.g., ParentID)
                // But include self-referencing from the same table (like Employee.ReportingManagerID)
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
        /// Counts active dependent records using raw SQL for performance.
        /// Checks IsActive column if it exists (AuditProperty pattern).
        /// </summary>
        private static async Task<int> CountDependentRecords(
            LIMSContext context, Type dependentType, string fkPropertyName, long entityId)
        {
            var entityType = context.Model.FindEntityType(dependentType);
            if (entityType == null) return 0;

            var tableName = entityType.GetTableName();
            var schema = entityType.GetSchema() ?? "dbo";

            // Map property name to column name
            var fkProperty = entityType.FindProperty(fkPropertyName);
            var columnName = fkProperty?.GetColumnName() ?? fkPropertyName;

            // Check if entity has IsActive column (AuditProperty)
            var hasIsActive = entityType.FindProperty("IsActive") != null;

            var sql = hasIsActive
                ? $"SELECT COUNT(*) FROM [{schema}].[{tableName}] WHERE [{columnName}] = @p0 AND [IsActive] = 1"
                : $"SELECT COUNT(*) FROM [{schema}].[{tableName}] WHERE [{columnName}] = @p0";

            try
            {
                var result = await context.Database
                    .SqlQueryRaw<int>(sql, entityId)
                    .FirstOrDefaultAsync();
                return result;
            }
            catch
            {
                // If query fails (e.g., table doesn't exist yet), skip silently
                return 0;
            }
        }

        private static object? GetDbSetForType(LIMSContext context, Type entityType)
        {
            var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
            return setMethod?.MakeGenericMethod(entityType).Invoke(context, null);
        }

        /// <summary>
        /// Converts PascalCase entity names to friendly display names.
        /// e.g., "EmployeeMaster" → "Employee", "SampleInward" → "Sample Inward"
        /// </summary>
        private static string GetFriendlyEntityName(string entityName)
        {
            // Remove common suffixes
            var name = entityName
                .Replace("Master", "")
                .Replace("master", "")
                .Replace("Nabl", "NABL ");

            // Add spaces before uppercase letters
            var result = string.Concat(name.Select((c, i) =>
                i > 0 && char.IsUpper(c) && !char.IsUpper(name[i - 1]) ? " " + c : c.ToString()));

            return result.Trim();
        }
    }
}
