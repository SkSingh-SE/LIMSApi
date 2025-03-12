using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IFileUploadRepository
    {
        Task<UploadFile> UploadFileAsync(UploadFile file);
        Task<UploadFile> GetFileAsync(long id);
    }
}
