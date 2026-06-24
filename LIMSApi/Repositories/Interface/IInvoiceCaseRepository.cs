using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IInvoiceCaseRepository
    {
        Task AddInvoiceCase(InvoiceCase model);
        Task UpdateInvoiceCase(InvoiceCase model);
        Task<InvoiceCase> GetInvoiceCaseById(long id);
        Task<PagedResponse<object>> GetAllInvoiceCases(PageFilter filter);

        Task<List<DropdwonSelector>> GetInvoiceCaseDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByFinancialAndName(long? financialYearId, string name);
        Task<bool> ExistsByFinancialYearAndTest(long? financialYearId, long laboratoryTestId);
        Task<bool> ExistsByFinancialYearAndTestNotId(long? financialYearId, long laboratoryTestId, long excludeId);

        /// <summary>All active InvoiceCase year-versions for a LaboratoryTest, prices included, newest EffectiveFrom first.</summary>
        Task<List<InvoiceCase>> GetByLabTest(long laboratoryTestId);

        /// <summary>Upserts all year-versions for a LaboratoryTest atomically (soft-deletes removed versions).</summary>
        Task SaveAllVersions(long laboratoryTestId, List<InvoiceCase> incoming);

        /// <summary>FinancialYear whose StartDate..EndDate contains the given date. Used to derive FinancialYearId from EffectiveFrom.</summary>
        Task<long?> GetFinancialYearIdForDate(DateTime date);

        /// <summary>Id of the FinancialYear flagged IsCurrent (default FY) — drives the ★ Current badge.</summary>
        Task<long?> GetCurrentFinancialYearId();
    }
}
