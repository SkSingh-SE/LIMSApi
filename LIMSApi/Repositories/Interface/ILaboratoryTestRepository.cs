using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ILaboratoryTestRepository
    {
        Task AddTestMethod(LaboratoryTest model);
        Task UpdateTestMethod(LaboratoryTest model);
        Task DeleteTestMethod(long id);
        Task<LaboratoryTest> GetTestMethodById(long id);
        Task<PagedResponse<object>> GetAllTestMethods(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetGeneralTestMethodDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetChemicalTestMethodDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<object>> GetTestCases(long labTestId);
        Task<List<PricingTemplateRowDto>> GetPricingTemplate(long labTestId, long? analysisTypeId);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<List<string>> GetDistinctTestNames(string? searchTerm, int pageSize);
    }
}
