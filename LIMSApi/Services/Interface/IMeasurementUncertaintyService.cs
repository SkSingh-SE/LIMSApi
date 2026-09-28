using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IMeasurementUncertaintyService
    {
        Task CreateUncertainty(MeasurementUncertaintyCreateDto dto);
        Task ModifyUncertainty(MeasurementUncertaintyUpdateDto dto);
        Task RemoveUncertainty(long id);
        Task<MeasurementUncertaintyDetailDto> GetUncertaintyDetails(long id);
        Task<PagedResponse<object>> FetchUncertaintyList(MeasurementUncertaintyListRequest request);
        Task<bool> ToggleUncertaintyStatus(long id);
        Task<List<DropdwonSelector>> GetUncertaintyDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<MeasurementUncertaintyValidateResponse> ValidateUncertainty(MeasurementUncertaintyValidateRequest request);
    }
}
