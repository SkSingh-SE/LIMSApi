using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaterialTestMappingController: ControllerBase
    {
        private readonly IMaterialTestMappingService _service;

        public MaterialTestMappingController(IMaterialTestMappingService materialTestService)
        {
            _service = materialTestService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List(PageFilter filter)
        {
            return Ok(await _service.GetAllMappings(filter));
        }



        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecificationHeader>> GetDetails(long id)
        {
            var entity = await _service.GetMapping(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> Update(MaterialTestMapping model)
        {
            await _service.UpdateMapping(model);
            return Ok(new
            {
                status = "success",
                message = $"Material Test Mapping updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> PostSpecificationHeader(MaterialTestMapping model)
        {
            await _service.CreateMapping(model);
            return Ok(new
            {
                status = "success",
                message = $"Material Test Mapping created successfully."
            });
        }
        [HttpGet("AutoSuggest")]
        public async Task<IActionResult> AutoSuggestTestMethods([FromQuery] TestSuggestionRequest request)
        {
            var suggestions = await _service.GetSuggestedTestsAsync(request);

            if (suggestions == null || !suggestions.Any())
                return NoContent();

            return Ok(suggestions);
        }

    }
}
