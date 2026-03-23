using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Auth enforced globally by Fix #1. No [AllowAnonymous] = protected.
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileService;
        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileService = fileUploadService;
        }

        [HttpPost("uploadFile")]
        [RequestSizeLimit(FileUploadValidator.MaxFileSizeBytes)] // Kestrel rejects oversized before reading
        public async Task<IActionResult> UploadFile(IFormFile file, int fileType, int? year)
        {
            // Early validation at controller level for fast feedback
            var validationError = await FileUploadValidator.ValidateAsync(file);
            if (validationError != null)
                return BadRequest(new { success = false, message = validationError });

            if (!Enum.IsDefined(typeof(FileType), fileType))
                return BadRequest(new { success = false, message = "Invalid file type category." });

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
