using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IFileUploadService
    {
        Task UploadFileAsync(IFormFile file, FileType fileType, int? year );
        Task<UploadFile> GetFileAsync(long id);
    }
}
