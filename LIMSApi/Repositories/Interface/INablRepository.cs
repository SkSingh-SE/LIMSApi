using LIMSApi.Dtos;

namespace LIMSApi.Repositories.Interface
{
    public interface INablRepository
    {
        Task<PagedResponse<object>> GetAll(string formType, PageFilter filter);
        Task<object?> GetById(string formType, long id);
        Task<long> Add(string formType, object model);
        Task Update(string formType, object model);
        Task Delete(string formType, long id);
    }
}
