using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICustomerService
    {
        Task CreateCustomer(Customer model);
        Task ModifyCustomer(Customer model);
        Task RemoveCustomer(long id);
        Task<Customer> GetCustomerDetails(long id);
        Task<PagedResponse<object>> FetchCustomerList(PageFilter filter);
        Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize);
        Task VerifyCustomer(long id, bool status);

        // Change request (Level 2 field approval)
        Task<List<CustomerChangeRequestResponseDto>> GetChangeRequests(long customerId);
        Task<CustomerChangeRequestResponseDto?> GetPendingChangeRequest(long customerId);
        Task ApplyChangeRequest(long changeRequestId);
        Task RejectChangeRequest(long changeRequestId, string? reason);
        Task DirectReviewChangeRequest(ReviewChangeRequestDto dto);
    }
}
