using LIMSApi.Dtos;

namespace LIMSApi.ServiceWORepo
{
    public interface ICustomerAmendmentService
    {
        Task<AmendmentTokenValidationResponse> ValidateTokenAsync(Guid token);
        Task<long> CreateAmendmentRequestAsync(CustomerAmendmentRequestDto dto);
    }

}
