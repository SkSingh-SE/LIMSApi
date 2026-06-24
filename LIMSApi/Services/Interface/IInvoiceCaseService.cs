using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IInvoiceCaseService
    {
        Task CreateInvoiceCase(InvoiceCase model);
        Task ModifyInvoiceCase(InvoiceCase model);
        Task RemoveInvoiceCase(long id);
        Task<InvoiceCase> GetInvoiceCaseDetails(long id);
        Task<PagedResponse<object>> FetchInvoiceCaseList(PageFilter filter);

        /// <summary>All year-versions for a LaboratoryTest, ★Current flagged on the default-FY version.</summary>
        Task<InvoiceCaseByLabTestDto> GetByLabTest(long laboratoryTestId);

        /// <summary>Upserts all year-versions for a LaboratoryTest in one transaction.</summary>
        Task SaveAllInvoiceCases(SaveAllInvoiceCaseDto dto);

    }
}
