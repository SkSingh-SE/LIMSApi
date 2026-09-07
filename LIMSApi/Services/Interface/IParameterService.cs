using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IParameterService
    {
        Task CreateParameter(ParameterMaster model);
        Task ModifyParameter(ParameterMaster model);
        Task RemoveParameter(long id);
        Task<ParameterMaster> GetParameterDetails(long id);
        Task<PagedResponse<object>> FetchChemicalParameterList(PageFilter filter);
        Task<PagedResponse<object>> FetchMechanicalParameterList(PageFilter filter);
        Task<PagedResponse<object>> ParameterList(PageFilter filter);

        Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize, string? elementTypes = null);
        Task<List<DropdwonSelector>> GetChemicalParameterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetMechanicalParameterDropdown(string? searchTerm, int pageNo, int pageSize);

        Task<PagedResponse<ParameterListItemDto>> GetAllParametersUnified(PageFilter filter);
        Task<ParameterDetailDto> GetParameterDetailById(long id);
        Task<long> CreateParameterUnified(ParameterCreateDto dto);
        Task ModifyParameterUnified(ParameterUpdateDto dto);
        Task<bool> ToggleParameterStatus(long id);
        Task<object> GetParameterTypesMetadata();

        Task<(bool IsValid, string? Error, IEnumerable<long> ParamIds)> ValidateFormulaForApi(string formula);
    }
}

