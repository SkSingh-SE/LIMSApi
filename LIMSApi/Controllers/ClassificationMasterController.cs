using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationMasterController : ControllerBase
    {
        private readonly IClassificationService _service;

        public ClassificationMasterController(IClassificationService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.Classification.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> ClassificationList([FromBody] PageFilter filter)
        {
            return Ok(await _service.FetchClassificationList(filter));
        }

        [RequirePermission(Permissions.Classification.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetClassification(long id)
        {
            var entity = await _service.GetClassificationDetails(id);
            return Ok(entity);
        }

        [RequirePermission(Permissions.Classification.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostClassification([FromBody] ClassificationMasterCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.CreateClassification(dto);
            return Ok(new
            {
                status = "success",
                message = $"Classification '{dto.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.Classification.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutClassification([FromBody] ClassificationMasterUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.ModifyClassification(dto);
            return Ok(new
            {
                status = "success",
                message = $"Classification '{dto.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.Classification.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var isActive = await _service.ToggleClassificationStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Classification {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [RequirePermission(Permissions.Classification.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteClassification(long id)
        {
            await _service.RemoveClassification(id);
            return Ok(new
            {
                status = "success",
                message = "Classification deactivated successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetClassificationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            var data = await _service.GetClassificationDropdown(searchTerm, pageNo, pageSize);
            return Ok(data);
        }
    }
}
