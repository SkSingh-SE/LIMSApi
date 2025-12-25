using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IFileUploadService
    {
        Task<UploadFile> UploadFileAsync(IFormFile file, FileType fileType, int? year, string? identifier );
        Task<UploadFile> GetFileAsync(long id);
        Task RemoveFileAsync(long id);
        string GetFilePath(string fileName, FileType fileType, int? year);
    }
}
