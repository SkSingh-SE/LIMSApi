using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class InvoiceCaseService : IInvoiceCaseService
    {
        private readonly IInvoiceCaseRepository _InvoiceCaseRepository;
        private readonly ILogger<InvoiceCaseService> _logger;

        public InvoiceCaseService(IInvoiceCaseRepository InvoiceCaseRepo, ILogger<InvoiceCaseService> logger)
        {
            _InvoiceCaseRepository = InvoiceCaseRepo;
            _logger = logger;
        }

        public async Task CreateInvoiceCase(InvoiceCase model)
        {
            // Check duplicate: same Financial Year + same Sub Group Test + same Analysis Type
            if (await _InvoiceCaseRepository.ExistsByFinancialYearAndTest(model.FinancialYearId, model.LaboratoryTestID, model.AnalysisTypeID))
                throw new InvalidOperationException($"Invoice Case already exists for this Financial Year!");

            foreach (var price in model.InvoiceCasePrices)
            {
                if (price != null)
                {
                    if (await _InvoiceCaseRepository.ExistsByFinancialAndName(model.FinancialYearId, price.Name))
                    {
                        throw new InvalidOperationException($"Same Invoice case {price.Name} already exists!");
                    }
                }
            }
            await _InvoiceCaseRepository.AddInvoiceCase(model);
            _logger.LogInformation("InvoiceCase '{InvoiceCaseName}' created successfully.", model.LaboratoryTestID);
        }

        public async Task ModifyInvoiceCase(InvoiceCase model)
        {
            if (model.ID == 0)
                throw new ArgumentException("InvoiceCase ID should not be empty!");

            var existingInvoiceCase = await _InvoiceCaseRepository.GetInvoiceCaseById(model.ID);
            if (existingInvoiceCase == null)
                throw new InvalidOperationException("InvoiceCase not found!");

            // Check duplicate on update: same Financial Year + same Sub Group Test + same Analysis Type (excluding self)
            if (await _InvoiceCaseRepository.ExistsByFinancialYearAndTestNotId(model.FinancialYearId, model.LaboratoryTestID, model.AnalysisTypeID, model.ID))
                throw new InvalidOperationException($"Invoice Case already exists for this Financial Year!");

            existingInvoiceCase.LaboratoryTestID = model.LaboratoryTestID;
            existingInvoiceCase.FinancialYearId = model.FinancialYearId;
            existingInvoiceCase.ModifiedOn = DateTime.UtcNow;

            // --- Remove missing Invoice Prices ---
            var toRemovePrice = existingInvoiceCase.InvoiceCasePrices
                .Where(x => !model.InvoiceCasePrices.Any(y => y.ID == x.ID))
                .ToList();

            foreach (var IP in toRemovePrice)
            {
                existingInvoiceCase.InvoiceCasePrices.Remove(IP);
            }

            // --- Add or Update Invoice Prices ---
            foreach (var incomingInvoices in model.InvoiceCasePrices)
            {
                var existingInvoicePrices = existingInvoiceCase.InvoiceCasePrices
                    .FirstOrDefault(x => x.ID == incomingInvoices.ID);

                if (existingInvoicePrices == null)
                {
                    existingInvoiceCase.InvoiceCasePrices.Add(incomingInvoices);
                }
                else
                {
                    // Update existing specification
                    existingInvoicePrices.InvoiceCaseConfigID = incomingInvoices.InvoiceCaseConfigID;
                    existingInvoicePrices.Name = incomingInvoices.Name;
                    existingInvoicePrices.AliasName = incomingInvoices.AliasName;
                    existingInvoicePrices.Price = incomingInvoices.Price;
                    existingInvoicePrices.ElementPrices = incomingInvoices.ElementPrices;
                }
            }

            await _InvoiceCaseRepository.UpdateInvoiceCase(existingInvoiceCase);
            _logger.LogInformation("InvoiceCase '{InvoiceCaseName}' updated successfully.", model.LaboratoryTestID);
        }


        public async Task RemoveInvoiceCase(long id)
        {
            var existingInvoiceCase = await _InvoiceCaseRepository.GetInvoiceCaseById(id);
            if (existingInvoiceCase == null)
                throw new InvalidOperationException("InvoiceCase not found!");

            existingInvoiceCase.IsActive = false;
            existingInvoiceCase.ModifiedOn = DateTime.UtcNow;

            await _InvoiceCaseRepository.UpdateInvoiceCase(existingInvoiceCase);
            _logger.LogInformation("InvoiceCase with ID '{InvoiceCaseId}' deleted successfully.", id);
        }

        public async Task<InvoiceCase> GetInvoiceCaseDetails(long id)
        {
            var classification = await _InvoiceCaseRepository.GetInvoiceCaseById(id);
            if (classification == null)
                throw new InvalidOperationException("InvoiceCase not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchInvoiceCaseList(PageFilter filter)
        {
            return await _InvoiceCaseRepository.GetAllInvoiceCases(filter);
        }

        public async Task<InvoiceCaseByLabTestDto> GetByLabTest(long laboratoryTestId, long? analysisTypeId)
        {
            if (laboratoryTestId == 0)
                throw new ArgumentException("LaboratoryTest is required!");

            var versions = await _InvoiceCaseRepository.GetByLabTest(laboratoryTestId, analysisTypeId);
            var currentFyId = await _InvoiceCaseRepository.GetCurrentFinancialYearId();
            var firstVersion = versions.FirstOrDefault();

            return new InvoiceCaseByLabTestDto
            {
                LaboratoryTestID = laboratoryTestId,
                LaboratoryTest = firstVersion?.LaboratoryTest?.Name,
                AnalysisTypeID = analysisTypeId,
                AnalysisTypeName = firstVersion?.AnalysisType?.Name,
                Versions = versions.Select(v => new InvoiceCaseVersionDto
                {
                    ID = v.ID,
                    EffectiveFrom = v.EffectiveFrom,
                    FinancialYearId = v.FinancialYearId,
                    FinancialYear = v.FinancialYearEntity?.Year,
                    DefaultPricingType = v.DefaultPricingType,
                    IsCurrentFy = currentFyId.HasValue && v.FinancialYearId == currentFyId,
                    AnalysisTypeID = v.AnalysisTypeID,
                    AnalysisTypeName = v.AnalysisType?.Name,
                    Prices = v.InvoiceCasePrices.Select(p => new InvoiceCasePriceDto
                    {
                        ID = p.ID,
                        InvoiceCaseConfigID = p.InvoiceCaseConfigID,
                        Name = p.Name,
                        AliasName = p.AliasName,
                        Price = p.Price,
                        ElementPrices = p.ElementPrices
                    }).ToList()
                }).ToList()
            };
        }

        public async Task SaveAllInvoiceCases(SaveAllInvoiceCaseDto dto)
        {
            if (dto.LaboratoryTestID == 0)
                throw new ArgumentException("Sub Group Test is required!");

            if (dto.Versions == null || !dto.Versions.Any())
                throw new InvalidOperationException("At least one year version is required!");

            // Validate each version
            foreach (var v in dto.Versions)
            {
                if (!v.FinancialYearId.HasValue)
                    throw new InvalidOperationException("Financial Year is required for every year version!");

                if (v.EffectiveFrom == default)
                    throw new InvalidOperationException("Effective From date is required for every year version!");

                if (v.Prices == null || !v.Prices.Any())
                    throw new InvalidOperationException($"Year version effective {v.EffectiveFrom:dd-MM-yyyy} must have at least one price row!");
            }

            // No two versions may share the same Effective From date
            var duplicate = dto.Versions
                .GroupBy(v => v.EffectiveFrom.Date)
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicate != null)
                throw new InvalidOperationException($"Duplicate Effective From date: {duplicate.Key:dd-MM-yyyy}. Each year version needs a distinct date.");

            // Map DTO → entities
            var incoming = new List<InvoiceCase>();
            foreach (var v in dto.Versions)
            {
                incoming.Add(new InvoiceCase
                {
                    ID = v.ID,
                    LaboratoryTestID = dto.LaboratoryTestID,
                    AnalysisTypeID = dto.AnalysisTypeID,
                    EffectiveFrom = v.EffectiveFrom,
                    FinancialYearId = v.FinancialYearId,
                    DefaultPricingType = v.DefaultPricingType,
                    InvoiceCasePrices = v.Prices.Select(p => new InvoiceCasePrice
                    {
                        ID = p.ID,
                        InvoiceCaseConfigID = p.InvoiceCaseConfigID,
                        Name = p.Name,
                        AliasName = p.AliasName,
                        Price = p.Price,
                        ElementPrices = p.ElementPrices
                    }).ToList()
                });
            }

            await _InvoiceCaseRepository.SaveAllVersions(dto.LaboratoryTestID, dto.AnalysisTypeID, incoming);
            _logger.LogInformation("InvoiceCase versions saved for LaboratoryTest {LabTestId} and AnalysisType {AnalysisTypeId}: {Count} year version(s).",
                dto.LaboratoryTestID, dto.AnalysisTypeID, incoming.Count);
        }

    }
}
