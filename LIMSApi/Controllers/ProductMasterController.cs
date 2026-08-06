using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductMasterController : ControllerBase
    {
        private readonly IProductMasterService _service;
        private readonly IFileUploadService _fileUploadService;

        public ProductMasterController(IProductMasterService service, IFileUploadService fileUploadService)
        {
            _service = service;
            _fileUploadService = fileUploadService;
        }

        [HttpPost("list")]
        [RequirePermission(Permissions.ProductMaster.Read)]
        public async Task<IActionResult> GetAll([FromBody] PageFilter filter)
        {
            var result = await _service.GetAllProductMasters(filter);
            return Ok(result);
        }

        [HttpGet("details/{id}")]
        [RequirePermission(Permissions.ProductMaster.Read)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetProductMasterById(id);
            if (result == null)
            {
                return NotFound(new { message = $"Product Master with ID {id} not found." });
            }
            return Ok(result);
        }

        [HttpPost("create")]
        [RequirePermission(Permissions.ProductMaster.Create)]
        public async Task<IActionResult> Create([FromBody] ProductMasterCreateDto dto)
        {
            var result = await _service.CreateProductMaster(dto);
            return Ok(result);
        }

        [HttpPut("update")]
        [RequirePermission(Permissions.ProductMaster.Update)]
        public async Task<IActionResult> Update([FromBody] ProductMasterUpdateDto dto)
        {
            var result = await _service.UpdateProductMaster(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [RequirePermission(Permissions.ProductMaster.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteProductMaster(id);
            return Ok(new { message = "Product Master deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown([FromQuery] string? searchTerm, [FromQuery] int pageNo = 0, [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetProductMasterDropdown(searchTerm, pageNo, pageSize);
            return Ok(result);
        }

        [HttpGet("grade-parameters/{gradeId}")]
        public async Task<IActionResult> GetGradeParameters(long gradeId)
        {
            var result = await _service.GetGradeParametersByGradeId(gradeId);
            if (result == null)
            {
                return NotFound(new { message = $"Specification Grade with ID {gradeId} not found." });
            }
            return Ok(result);
        }

        [HttpGet("prefix-options")]
        public async Task<IActionResult> GetPrefixOptions()
        {
            var result = await _service.GetPrefixOptions();
            return Ok(result);
        }

        [HttpPost("prefix-options/add")]
        public async Task<IActionResult> AddPrefixOption([FromBody] string prefix)
        {
            var result = await _service.AddPrefixOption(prefix);
            return Ok(new { success = result });
        }

        [HttpPost("upload-spec-file")]
        public async Task<IActionResult> UploadSpecFile(IFormFile file)
        {
            var uploadedFile = await _fileUploadService.UploadFileAsync(file, FileType.Product, DateTime.UtcNow.Year, null);
            return Ok(new { filePath = uploadedFile.FilePath, fileName = uploadedFile.OriginalFileName });
        }
    }
}
