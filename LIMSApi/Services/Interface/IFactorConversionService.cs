using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IFactorConversionService
    {
        Task CreateFactor(FactorConversionCreateDto dto);
        Task ModifyFactor(FactorConversionUpdateDto dto);
        Task RemoveFactor(long id);
        Task<FactorConversionDetailDto> GetFactorDetails(long id);
        Task<PagedResponse<object>> FetchFactorList(FactorConversionListRequest request);
        Task<bool> ToggleFactorStatus(long id);
        Task<List<DropdwonSelector>> GetFactorDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<FactorValidateResponse> ValidateFactor(FactorValidateRequest request);
    }
}
