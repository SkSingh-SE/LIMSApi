using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface ISpecificationVersionService
    {
        Task<PagedResponse<SpecificationVersionListItemDto>> GetPagedVersionsAsync(SpecificationVersionFilterDto filter);
        Task<SpecificationVersionDetailDto> GetVersionDetailsAsync(long id);
        Task<SpecificationVersionListItemDto> CreateVersionAsync(SpecificationVersionCreateDto dto);
        Task<SpecificationVersionListItemDto> UpdateVersionAsync(long id, SpecificationVersionUpdateDto dto);
        Task<bool> ActivateVersionAsync(long id);
        Task<bool> SupersedeVersionAsync(long id, DateTime? supersededDate = null);
        Task<bool> WithdrawVersionAsync(long id);
        Task<bool> SetDefaultVersionAsync(long id);
        Task<List<SpecificationVersionParameterDto>> GetVersionParametersAsync(long id);
        Task<bool> SaveVersionParametersAsync(long id, SaveSpecificationVersionParametersDto dto);
        Task<List<SpecificationVersionDropdownDto>> GetDropdownBySpecificationIdAsync(long specId, bool includeAll = false);
        Task<bool> DeleteVersionAsync(long id);
    }
}
