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
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                FileType fileTypes;
                if (!Enum.IsDefined(typeof(FileType), fileType))
                    return BadRequest("Invalid file type.");

                fileTypes = (FileType)fileType;

                await _fileService.UploadFileAsync(file, fileTypes, year);
                return Ok(new { message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File Upload Error: {ex.Message}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpGet("files/{id}")]
        public async Task<IActionResult> GetFileById(int id)
        {
            var file = await _fileService.GetFileAsync(id);
            return Ok(file);
        }
    }
}
