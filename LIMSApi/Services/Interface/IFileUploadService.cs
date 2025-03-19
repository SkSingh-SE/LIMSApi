using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IFileUploadService
    {
        Task<UploadFile> UploadFileAsync(IFormFile file, FileType fileType, int? year );
        Task<UploadFile> GetFileAsync(long id);
    }
}
