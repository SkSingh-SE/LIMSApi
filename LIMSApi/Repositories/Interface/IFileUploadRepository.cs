using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IFileUploadRepository
    {
        Task<UploadFile> UploadFile(UploadFile file);
        Task<UploadFile> GetUploadFile(long id);
        Task RemoveFile(UploadFile file);
    }
}
