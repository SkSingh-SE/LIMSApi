using System.Text.Json;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TestLayoutTemplateService : ITestLayoutTemplateService
    {
        private readonly ITestLayoutTemplateRepository _repository;
        private readonly ILogger<TestLayoutTemplateService> _logger;
        private LoggedInUserDTO loggedInUser;

        public TestLayoutTemplateService(
            ITestLayoutTemplateRepository repository,
            ILogger<TestLayoutTemplateService> logger)
        {
            _repository = repository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateLayoutTemplate(TestLayoutTemplate model)
        {
            Validate(model);
            EnrichSummaryFromConfig(model);

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;
            model.IsActive = true;

            if (string.IsNullOrWhiteSpace(model.LayoutCode))
            {
                model.LayoutCode = model.IsGlobalDefault
                    ? $"TL-GLOBAL-{DateTime.UtcNow:yyyyMMddHHmmss}"
                    : $"TL-TEST{model.LaboratoryTestID}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            }

            await _repository.AddLayoutTemplate(model);

            if (model.MappedLeafNodes != null && model.MappedLeafNodes.Any())
            {
                await _repository.SyncMappedLeafNodes(model.ID, model.MappedLeafNodes);
            }
            else if (model.MappedTestIDs != null && model.MappedTestIDs.Any())
            {
                await _repository.SyncMappedTests(model.ID, model.MappedTestIDs);
            }

            if (model.IsDefault)
            {
                await _repository.ResetDefaultLayouts(model.LaboratoryTestID, model.LaboratoryTestAnalysisTypeID, model.ID);
            }

            _logger.LogInformation("Layout template '{LayoutName}' (ID: {ID}) created successfully.", model.LayoutName, model.ID);
        }

        public async Task ModifyLayoutTemplate(TestLayoutTemplate model)
        {
            if (model.ID <= 0)
                throw new ArgumentException("Layout template ID is required for update.");

            Validate(model);
            EnrichSummaryFromConfig(model);

            var existing = await _repository.GetLayoutTemplateById(model.ID);
            if (existing == null)
                throw new InvalidOperationException($"Layout template with ID {model.ID} not found.");

            existing.LayoutName = model.LayoutName;
            existing.LayoutCode = model.LayoutCode;
            existing.Revision = model.Revision;
            existing.Version = model.Version;
            existing.Status = model.Status;
            existing.IsDefault = model.IsDefault;
            existing.IsGlobalDefault = model.IsGlobalDefault;
            existing.LaboratoryTestID = model.LaboratoryTestID;
            existing.LaboratoryTestSubGroupID = model.LaboratoryTestSubGroupID;
            existing.LaboratoryTestAnalysisTypeID = model.LaboratoryTestAnalysisTypeID;
            existing.LeafNode = model.LeafNode;
            existing.ApplyToLeaf = model.ApplyToLeaf;
            existing.SectionCount = model.SectionCount;
            existing.ArchetypeSummary = model.ArchetypeSummary;
            existing.LayoutConfigJson = model.LayoutConfigJson;
            existing.OutputPayloadSchemaJson = model.OutputPayloadSchemaJson;

            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateLayoutTemplate(existing);

            if (model.MappedLeafNodes != null)
            {
                await _repository.SyncMappedLeafNodes(existing.ID, model.MappedLeafNodes);
            }
            else if (model.MappedTestIDs != null)
            {
                await _repository.SyncMappedTests(existing.ID, model.MappedTestIDs);
            }

            if (existing.IsDefault)
            {
                await _repository.ResetDefaultLayouts(existing.LaboratoryTestID, existing.LaboratoryTestAnalysisTypeID, existing.ID);
            }

            _logger.LogInformation("Layout template '{LayoutName}' (ID: {ID}) updated successfully.", existing.LayoutName, existing.ID);
        }

        public async Task RemoveLayoutTemplate(long id)
        {
            var existing = await _repository.GetLayoutTemplateById(id);
            if (existing == null)
                throw new InvalidOperationException($"Layout template with ID {id} not found.");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.DeleteLayoutTemplate(existing);
            _logger.LogInformation("Layout template ID {ID} soft-deleted.", id);
        }

        public async Task<TestLayoutTemplate?> GetLayoutTemplateDetails(long id)
        {
            var entity = await _repository.GetLayoutTemplateById(id);
            if (entity != null)
            {
                entity.MappedTestIDs = await _repository.GetMappedTestIds(entity.ID);
                entity.MappedLeafNodes = await _repository.GetMappedLeafNodes(entity.ID);
            }
            return entity;
        }

        public async Task<TestLayoutTemplate?> GetLayoutByHierarchy(long? testId, long? subGroupId, long? analysisTypeId, string? leafNode, long? testMethodId = null, string? testMethodName = null)
        {
            var entity = await _repository.GetLayoutByTestHierarchy(testId, subGroupId, analysisTypeId, leafNode, testMethodId, testMethodName);
            if (entity != null)
            {
                entity.MappedTestIDs = await _repository.GetMappedTestIds(entity.ID);
                entity.MappedLeafNodes = await _repository.GetMappedLeafNodes(entity.ID);
            }
            return entity;
        }

        public async Task<PagedResponse<object>> FetchLayoutTemplateList(PageFilter filter)
        {
            return await _repository.GetAllLayoutTemplates(filter);
        }

        public async Task<List<DropdwonSelector>> GetLayoutTemplateDropdown(long? testId, string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetLayoutDropdown(testId, searchTerm, pageNo, pageSize);
        }

        private static void Validate(TestLayoutTemplate model)
        {
            if (!model.IsGlobalDefault && (!model.LaboratoryTestID.HasValue || model.LaboratoryTestID.Value <= 0))
            {
                if (model.MappedTestIDs == null || !model.MappedTestIDs.Any())
                {
                    throw new ArgumentException("Laboratory Test ID or Global Default flag is required.");
                }
            }

            if (string.IsNullOrWhiteSpace(model.LayoutName))
                throw new ArgumentException("Layout Name is mandatory.");

            if (string.IsNullOrWhiteSpace(model.LayoutConfigJson))
                throw new ArgumentException("Layout Configuration JSON is mandatory.");
        }

        private static void EnrichSummaryFromConfig(TestLayoutTemplate model)
        {
            try
            {
                using var doc = JsonDocument.Parse(model.LayoutConfigJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("sections", out var sectionsElem) && sectionsElem.ValueKind == JsonValueKind.Array)
                {
                    model.SectionCount = sectionsElem.GetArrayLength();

                    var types = new HashSet<string>();
                    foreach (var sec in sectionsElem.EnumerateArray())
                    {
                        if (sec.TryGetProperty("type", out var typeElem))
                        {
                            var t = typeElem.GetString();
                            if (!string.IsNullOrWhiteSpace(t))
                            {
                                types.Add(t);
                            }
                        }
                    }

                    if (types.Count > 0)
                    {
                        model.ArchetypeSummary = string.Join(", ", types);
                    }
                }
            }
            catch
            {
                // Fallback to existing or 0 if JSON parse encounters partial malformed string
            }
        }
    }
}
