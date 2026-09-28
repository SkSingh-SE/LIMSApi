using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class ExecutionLayoutService : IExecutionLayoutService
    {
        private readonly IExecutionLayoutRepository _repository;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public ExecutionLayoutService(IExecutionLayoutRepository repository, LIMSContext context)
        {
            _repository = repository;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private static string NormalizeCode(string code) =>
            Regex.Replace(code.Trim().ToUpperInvariant(), @"\s+", "_");

        private static readonly HashSet<string> AllowedRendererTypes = new(StringComparer.Ordinal)
        {
            "ObservationMatrix", "MultiSpecimen", "MultiReading", "ParameterTable",
            "Qualitative", "Calculation", "Graph"
        };

        public static readonly Dictionary<string, SectionTypeMetaDto> CanonicalSectionTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Preparation"] = new SectionTypeMetaDto
            {
                SectionType = "Preparation",
                DisplayName = "Sample Preparation",
                Description = "Sample preparation instructions, checklist, and verification items",
                DefaultPresentationStyle = "ChecklistForm",
                AllowedReferenceTypes = new List<string> { "None", "ConditionMaster" }
            },
            ["Conditions"] = new SectionTypeMetaDto
            {
                SectionType = "Conditions",
                DisplayName = "Environmental & Test Conditions",
                Description = "Environmental and testing prerequisite condition parameters",
                DefaultPresentationStyle = "DefaultGrid",
                AllowedReferenceTypes = new List<string> { "None", "ConditionMaster" }
            },
            ["Equipment"] = new SectionTypeMetaDto
            {
                SectionType = "Equipment",
                DisplayName = "Equipment Requirements",
                Description = "Required apparatus, equipment type, and calibration criteria",
                DefaultPresentationStyle = "DefaultGrid",
                AllowedReferenceTypes = new List<string> { "None", "EquipmentRequirementMaster" }
            },
            ["Parameters"] = new SectionTypeMetaDto
            {
                SectionType = "Parameters",
                DisplayName = "Test Parameters",
                Description = "Direct scientific parameters and measurable properties",
                DefaultPresentationStyle = "ParameterTable",
                AllowedReferenceTypes = new List<string> { "None", "ParameterMaster" }
            },
            ["Observations"] = new SectionTypeMetaDto
            {
                SectionType = "Observations",
                DisplayName = "Observations & Readings",
                Description = "Multi-specimen or multi-reading raw observation matrix",
                DefaultPresentationStyle = "ObservationMatrix",
                AllowedReferenceTypes = new List<string> { "None", "ParameterMaster" }
            },
            ["Calculations"] = new SectionTypeMetaDto
            {
                SectionType = "Calculations",
                DisplayName = "Calculations & Formulas",
                Description = "Computed, derived, or formula-evaluated parameter values",
                DefaultPresentationStyle = "ParameterTable",
                AllowedReferenceTypes = new List<string> { "None", "ParameterMaster" }
            },
            ["Graph"] = new SectionTypeMetaDto
            {
                SectionType = "Graph",
                DisplayName = "Graph & Curves",
                Description = "Interactive chart display with explicit X/Y axis parameter bindings",
                DefaultPresentationStyle = "InteractiveChart",
                AllowedReferenceTypes = new List<string> { "None", "ParameterMaster", "GraphXAxis", "GraphYAxis", "GraphSeries" }
            },
            ["Factors"] = new SectionTypeMetaDto
            {
                SectionType = "Factors",
                DisplayName = "Conversion Factors",
                Description = "Multiplication/division factors and unit conversion constants",
                DefaultPresentationStyle = "DefaultGrid",
                AllowedReferenceTypes = new List<string> { "None", "FactorConversionMaster" }
            },
            ["MeasurementUncertainty"] = new SectionTypeMetaDto
            {
                SectionType = "MeasurementUncertainty",
                DisplayName = "Measurement Uncertainty",
                Description = "ISO 17025 Measurement uncertainty specifications and expanded uncertainties",
                DefaultPresentationStyle = "DefaultGrid",
                AllowedReferenceTypes = new List<string> { "None", "MeasurementUncertaintyMaster" }
            },
            ["AcceptanceCriteria"] = new SectionTypeMetaDto
            {
                SectionType = "AcceptanceCriteria",
                DisplayName = "Acceptance Criteria",
                Description = "Pass/Fail acceptance rules, standard specifications, and tolerance limits",
                DefaultPresentationStyle = "DecisionSummary",
                AllowedReferenceTypes = new List<string> { "None", "AcceptanceCriteriaMaster" }
            },
            ["Attachments"] = new SectionTypeMetaDto
            {
                SectionType = "Attachments",
                DisplayName = "Attachments & Spectra",
                Description = "Raw machine spectra, instrument data exports, and file attachments",
                DefaultPresentationStyle = "DefaultGrid",
                AllowedReferenceTypes = new List<string> { "None" }
            },
            ["Remarks"] = new SectionTypeMetaDto
            {
                SectionType = "Remarks",
                DisplayName = "Remarks & Notes",
                Description = "Structured execution observations, technician remarks, and anomaly notes",
                DefaultPresentationStyle = "StructuredNotes",
                AllowedReferenceTypes = new List<string> { "None" }
            }
        };

        private static readonly Dictionary<string, string> LegacySectionTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["General"] = "Remarks",
            ["Summary"] = "Remarks",
            ["Environment"] = "Conditions",
            ["Specimen"] = "Observations",
            ["Readings"] = "Observations",
            ["ChemicalComposition"] = "Parameters",
            ["StandardSamples"] = "Equipment",
            ["CBR_Readings"] = "Observations",
            ["CBR_Graph"] = "Graph",
            ["MoistureContent"] = "Observations"
        };

        public static readonly List<PresentationStyleMetaDto> CanonicalPresentationStyles = new()
        {
            new PresentationStyleMetaDto { Style = "DefaultGrid", DisplayName = "Default Grid", Description = "Standard tabular key-value or item grid presentation" },
            new PresentationStyleMetaDto { Style = "ParameterTable", DisplayName = "Parameter Table", Description = "Tabular parameter list with limits, units, and result entry" },
            new PresentationStyleMetaDto { Style = "ObservationMatrix", DisplayName = "Observation Matrix", Description = "Multi-specimen or multi-reading matrix grid for raw readings" },
            new PresentationStyleMetaDto { Style = "InteractiveChart", DisplayName = "Interactive Chart", Description = "Interactive 2D graph with pan/zoom and point inspection" },
            new PresentationStyleMetaDto { Style = "ChecklistForm", DisplayName = "Checklist Form", Description = "Sequential checkbox verification checklist" },
            new PresentationStyleMetaDto { Style = "DecisionSummary", DisplayName = "Decision Summary", Description = "Acceptance criteria comparison table with pass/fail badges" },
            new PresentationStyleMetaDto { Style = "StructuredNotes", DisplayName = "Structured Notes", Description = "Multi-line structured notes and technician remarks box" }
        };

        private static string? NormalizeRendererType(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var v = value.Trim();
            if (!AllowedRendererTypes.Contains(v))
                throw new ArgumentException($"Unsupported renderer type '{value}'. Allowed values: {string.Join(", ", AllowedRendererTypes)}.");
            return v;
        }

        private async Task<(bool ok, string? name, string? code, string? error)> ValidateReferenceAsync(string type, long? refId, string companyCode)
        {
            if (type == "None" || refId == null) return (true, null, null, null);
            long id = refId.Value;
            switch (type)
            {
                case "ParameterMaster":
                case "GraphXAxis":
                case "GraphYAxis":
                case "GraphSeries":
                    var p = await _context.ParameterMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (p == null) return (false, null, null, $"ParameterMaster ID {id} not found");
                    if (p.CompanyCode != companyCode) return (false, null, null, $"Cross-company reference rejected: ParameterMaster ID {id}");
                    return (p.IsActive, p.Name, p.Code, p.IsActive ? null : $"ParameterMaster '{p.Name}' (ID {id}) is inactive");
                case "ConditionMaster":
                    var c = await _context.ConditionMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (c == null) return (false, null, null, $"ConditionMaster ID {id} not found");
                    if (c.CompanyCode != companyCode) return (false, null, null, $"Cross-company reference rejected: ConditionMaster ID {id}");
                    return (c.IsActive, c.Name, c.Code, c.IsActive ? null : $"ConditionMaster '{c.Name}' (ID {id}) is inactive");
                case "EquipmentRequirementMaster":
                    var e = await _context.EquipmentRequirementMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (e == null) return (false, null, null, $"EquipmentRequirementMaster ID {id} not found");
                    if (e.CompanyCode != companyCode) return (false, null, null, $"Cross-company reference rejected: EquipmentRequirementMaster ID {id}");
                    return (e.IsActive, e.Name, e.Code, e.IsActive ? null : $"EquipmentRequirementMaster '{e.Name}' (ID {id}) is inactive");
                case "FactorConversionMaster":
                    var f = await _context.FactorConversionMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (f == null) return (false, null, null, $"FactorConversionMaster ID {id} not found");
                    if (f.CompanyCode != companyCode) return (false, null, null, $"Cross-company reference rejected: FactorConversionMaster ID {id}");
                    return (f.IsActive, f.Name, f.Code, f.IsActive ? null : $"FactorConversionMaster '{f.Name}' (ID {id}) is inactive");
                case "MeasurementUncertaintyMaster":
                    var m = await _context.MeasurementUncertaintyMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (m == null) return (false, null, null, $"MeasurementUncertaintyMaster ID {id} not found");
                    if (m.CompanyCode != companyCode) return (false, null, null, $"Cross-company reference rejected: MeasurementUncertaintyMaster ID {id}");
                    return (m.IsActive, m.Name, m.Code, m.IsActive ? null : $"MeasurementUncertaintyMaster '{m.Name}' (ID {id}) is inactive");
                case "AcceptanceCriteriaMaster":
                    var a = await _context.AcceptanceCriteriaMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (a == null) return (false, null, null, $"AcceptanceCriteriaMaster ID {id} not found");
                    if (a.CompanyCode != companyCode) return (false, null, null, $"Cross-company reference rejected: AcceptanceCriteriaMaster ID {id}");
                    return (a.IsActive, a.Name, a.Code, a.IsActive ? null : $"AcceptanceCriteriaMaster '{a.Name}' (ID {id}) is inactive");
                default:
                    return (false, null, null, $"Unknown ReferenceType '{type}'");
            }
        }

        public async Task CreateLayout(ExecutionLayoutCreateDto dto)
        {
            if (await _repository.ExistsByCode(NormalizeCode(dto.Code)))
                throw new ArgumentException($"Execution layout code '{dto.Code}' already exists.");
            await ValidateStructureAsync(dto.Sections, loggedInUser.CompanyCode);
            var model = new ExecutionLayoutMaster
            {
                Code = NormalizeCode(dto.Code),
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                RendererType = NormalizeRendererType(dto.RendererType),
                DisplayOrder = dto.DisplayOrder,
                Sections = dto.Sections.Select((s, si) => MapSection(s, si)).ToList()
            };
            await _repository.AddLayout(model);
        }

        public async Task ModifyLayout(ExecutionLayoutUpdateDto dto)
        {
            var existing = await _context.ExecutionLayoutMasters
                .FirstOrDefaultAsync(x => x.ID == dto.ID && x.CompanyCode == loggedInUser.CompanyCode)
                ?? throw new KeyNotFoundException($"Execution layout ID {dto.ID} not found.");

            if (await _repository.ExistsByCodeAndNotId(NormalizeCode(dto.Code), dto.ID))
                throw new ArgumentException($"Execution layout code '{dto.Code}' already exists.");

            await ValidateStructureAsync(dto.Sections, loggedInUser.CompanyCode);

            using var tx = await _context.Database.BeginTransactionAsync();

            existing.Code = NormalizeCode(dto.Code);
            existing.Name = dto.Name.Trim();
            existing.Description = dto.Description?.Trim();
            existing.RendererType = NormalizeRendererType(dto.RendererType);
            existing.DisplayOrder = dto.DisplayOrder;
            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;

            // Delete old sections and their items to free the unique constraint (ExecutionLayoutID, SectionCode)
            var oldSections = await _context.ExecutionLayoutSections
                .Include(s => s.Items)
                .Where(s => s.ExecutionLayoutID == existing.ID)
                .ToListAsync();

            if (oldSections.Count > 0)
            {
                _context.ExecutionLayoutSections.RemoveRange(oldSections);
                await _context.SaveChangesAsync();
            }

            // Map and add new sections
            var newSections = dto.Sections.Select((s, si) =>
            {
                var sec = MapSection(s, si);
                sec.ExecutionLayoutID = existing.ID;
                sec.CompanyCode = loggedInUser.CompanyCode;
                sec.CreatedBy = loggedInUser.EmployeeID;
                sec.CreatedOn = DateTime.UtcNow;
                foreach (var it in sec.Items)
                {
                    it.CompanyCode = loggedInUser.CompanyCode;
                    it.CreatedBy = loggedInUser.EmployeeID;
                    it.CreatedOn = DateTime.UtcNow;
                }
                return sec;
            }).ToList();

            await _context.ExecutionLayoutSections.AddRangeAsync(newSections);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
        }

        private ExecutionLayoutSection MapSection(ExecutionLayoutSectionDto s, int si) => new()
        {
            SectionCode = NormalizeCode(s.SectionCode),
            SectionName = s.SectionName.Trim(),
            SectionType = s.SectionType,
            DisplayOrder = s.DisplayOrder > 0 ? s.DisplayOrder : si,
            IsVisible = s.IsVisible, IsCollapsible = s.IsCollapsible, IsRequired = s.IsRequired,
            Items = s.Items.Select((it, ii) => MapItem(it, ii)).ToList()
        };

        private ExecutionLayoutItem MapItem(ExecutionLayoutItemDto it, int ii) => new()
        {
            ReferenceType = it.ReferenceType,
            ReferenceID = it.ReferenceID,
            DisplayLabel = string.IsNullOrWhiteSpace(it.DisplayLabel) ? null : it.DisplayLabel.Trim(),
            DisplayOrder = it.DisplayOrder > 0 ? it.DisplayOrder : ii,
            IsVisible = it.IsVisible, IsEditable = it.IsEditable, IsRequired = it.IsRequired
        };

        private async Task ValidateStructureAsync(List<ExecutionLayoutSectionDto> sections, string cc)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in sections)
            {
                if (string.IsNullOrWhiteSpace(s.SectionCode))
                    throw new ArgumentException("SectionCode is required.");
                if (string.IsNullOrWhiteSpace(s.SectionName))
                    throw new ArgumentException("SectionName is required.");
                if (string.IsNullOrWhiteSpace(s.SectionType))
                    throw new ArgumentException("SectionType is required.");

                if (!seen.Add(NormalizeCode(s.SectionCode)))
                    throw new ArgumentException($"Duplicate section code '{s.SectionCode}' in layout.");

                SectionTypeMetaDto? secMeta = null;
                if (CanonicalSectionTypes.TryGetValue(s.SectionType, out var canonical))
                {
                    secMeta = canonical;
                }
                else if (LegacySectionTypeMap.TryGetValue(s.SectionType, out var mapped) && CanonicalSectionTypes.TryGetValue(mapped, out var legacyMapped))
                {
                    secMeta = legacyMapped;
                }
                else
                {
                    throw new ArgumentException($"Unsupported SectionType '{s.SectionType}'. Allowed values: {string.Join(", ", CanonicalSectionTypes.Keys)}.");
                }

                int graphXCount = 0;
                int graphYCount = 0;

                foreach (var it in s.Items)
                {
                    if (!secMeta.AllowedReferenceTypes.Contains(it.ReferenceType, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException($"ReferenceType '{it.ReferenceType}' is not allowed in section '{s.SectionName}' of type '{s.SectionType}'. Allowed reference types: {string.Join(", ", secMeta.AllowedReferenceTypes)}.");
                    }

                    if (string.Equals(it.ReferenceType, "GraphXAxis", StringComparison.OrdinalIgnoreCase))
                    {
                        graphXCount++;
                        if (graphXCount > 1)
                            throw new ArgumentException($"Section '{s.SectionName}' has multiple GraphXAxis items. A graph section must have at most one X-axis.");
                    }
                    if (string.Equals(it.ReferenceType, "GraphYAxis", StringComparison.OrdinalIgnoreCase))
                    {
                        graphYCount++;
                        if (graphYCount > 1)
                            throw new ArgumentException($"Section '{s.SectionName}' has multiple GraphYAxis items. A graph section must have at most one Y-axis.");
                    }

                    var (ok, _, _, err) = await ValidateReferenceAsync(it.ReferenceType, it.ReferenceID, cc);
                    if (!ok) throw new ArgumentException(err);
                }
            }
        }

        public async Task<ExecutionLayoutMetadataDto> GetMetadataAsync()
        {
            var meta = new ExecutionLayoutMetadataDto
            {
                SectionTypes = CanonicalSectionTypes.Values.ToList(),
                PresentationStyles = CanonicalPresentationStyles,
                AllowedRendererTypes = AllowedRendererTypes.ToList()
            };
            return await Task.FromResult(meta);
        }

        public async Task<ExecutionLayoutDto> GetLayoutDetails(long id)
        {
            var e = await _repository.GetByIdWithStructure(id)
                ?? throw new KeyNotFoundException($"Execution layout ID {id} not found.");
            return await MapToDto(e, validate: true);
        }

        public async Task<ExecutionLayoutDto> ValidateLayout(long id)
        {
            var e = await _repository.GetByIdWithStructure(id)
                ?? throw new KeyNotFoundException($"Execution layout ID {id} not found.");
            return await MapToDto(e, validate: true);
        }

        private async Task<ExecutionLayoutDto> MapToDto(ExecutionLayoutMaster e, bool validate)
        {
            var errors = new List<string>();
            var sections = new List<ExecutionLayoutSectionDto>();
            foreach (var s in e.Sections.OrderBy(x => x.DisplayOrder))
            {
                var items = new List<ExecutionLayoutItemDto>();
                foreach (var it in s.Items.OrderBy(x => x.DisplayOrder))
                {
                    string? refName = null;
                    string? refCode = null;
                    bool refActive = true;
                    if (it.ReferenceType != "None" && it.ReferenceID.HasValue)
                    {
                        var (ok, name, code, err) = await ValidateReferenceAsync(it.ReferenceType, it.ReferenceID, e.CompanyCode);
                        refName = name;
                        refCode = code;
                        refActive = ok;
                        if (!ok && validate) errors.Add(err!);
                    }
                    items.Add(new ExecutionLayoutItemDto
                    {
                        ID = it.ID, ExecutionLayoutSectionID = it.ExecutionLayoutSectionID,
                        ReferenceType = it.ReferenceType, ReferenceID = it.ReferenceID,
                        DisplayLabel = it.DisplayLabel, DisplayOrder = it.DisplayOrder,
                        IsVisible = it.IsVisible, IsEditable = it.IsEditable, IsRequired = it.IsRequired,
                        IsActive = it.IsActive, ReferenceName = refName, ReferenceCode = refCode, ReferenceIsActive = refActive
                    });
                }
                string presentationStyle = "DefaultGrid";
                string sectionDisplayName = s.SectionName;
                if (CanonicalSectionTypes.TryGetValue(s.SectionType, out var canSec))
                {
                    presentationStyle = canSec.DefaultPresentationStyle;
                    sectionDisplayName = canSec.DisplayName;
                }
                else if (LegacySectionTypeMap.TryGetValue(s.SectionType, out var legSec) && CanonicalSectionTypes.TryGetValue(legSec, out var canSec2))
                {
                    presentationStyle = canSec2.DefaultPresentationStyle;
                    sectionDisplayName = canSec2.DisplayName;
                }

                sections.Add(new ExecutionLayoutSectionDto
                {
                    ID = s.ID, ExecutionLayoutID = s.ExecutionLayoutID, SectionCode = s.SectionCode,
                    SectionName = s.SectionName, SectionType = s.SectionType, DisplayOrder = s.DisplayOrder,
                    IsVisible = s.IsVisible, IsCollapsible = s.IsCollapsible, IsRequired = s.IsRequired,
                    IsActive = s.IsActive, PresentationStyle = presentationStyle,
                    SectionTypeDisplayName = sectionDisplayName,
                    Items = items
                });
            }
            var modName = await GetEmployeeName(e.ModifiedBy);
            var creName = await GetEmployeeName(e.CreatedBy);
            return new ExecutionLayoutDto
            {
                ID = e.ID, Code = e.Code, Name = e.Name, Description = e.Description,
                RendererType = e.RendererType, DisplayOrder = e.DisplayOrder, IsActive = e.IsActive,
                CompanyCode = e.CompanyCode, CreatedByName = creName, CreatedOn = e.CreatedOn,
                ModifiedByName = modName, ModifiedOn = e.ModifiedOn, Sections = sections
            };
        }

        private async Task<string?> GetEmployeeName(long? id)
        {
            if (id == null) return null;
            return await _context.EmployeeMasters.AsNoTracking()
                .Where(x => x.ID == id).Select(x => (string?)x.Name).FirstOrDefaultAsync();
        }

        public async Task<PagedResponse<object>> FetchLayoutList(ExecutionLayoutListRequest request) =>
            await _repository.GetPagedLayouts(request);

        public async Task<bool> ToggleLayoutStatus(long id)
        {
            var e = await _repository.GetById(id)
                ?? throw new KeyNotFoundException($"Execution layout ID {id} not found.");
            e.IsActive = !e.IsActive;
            e.ModifiedBy = loggedInUser.EmployeeID;
            e.ModifiedOn = DateTime.UtcNow;
            await _repository.UpdateLayout(e);
            return e.IsActive;
        }

        public async Task<List<DropdwonSelector>> GetLayoutDropdown(string? searchTerm, int pageNo, int pageSize) =>
            await _repository.GetDropdown(searchTerm, pageNo, pageSize);
    }
}
