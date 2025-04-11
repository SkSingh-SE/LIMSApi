using LIMSApi.Dtos;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileService;
        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileService = fileUploadService;
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file,  int fileType, int? year)
        {
            FileType fileTypes = (FileType)fileType;
            var uploadFile = await _fileService.UploadFileAsync(file, fileTypes, year, "");
            return Ok(uploadFile);
        }


        [HttpGet("files/{id}")]
        public async Task<IActionResult> GetFileById(int id)
        {
            var file = await _fileService.GetFileAsync(id);
            return Ok(file);
        }
    }
}
