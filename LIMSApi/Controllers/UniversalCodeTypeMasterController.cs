using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversalCodeTypeMasterController : ControllerBase
    {
        private readonly IUniversalCodeTypeService _universalCodeTypeService;

        public UniversalCodeTypeMasterController(IUniversalCodeTypeService universalService)
        {
            _universalCodeTypeService = universalService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> UniversalCodeTypeList(PageFilter filter)
        {
            return Ok(await _universalCodeTypeService.FetchUniversalCodeTypeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<UniversalCodeTypeMaster>> GetUniversalCodeTypeMaster(long id)
        {
            var entity = await _universalCodeTypeService.GetUniversalCodeTypeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutUniversalCodeTypeMaster(UniversalCodeTypeMaster model)
        {
            await _universalCodeTypeService.ModifyUniversalCodeType(model);
            return Ok($"UniversalCodeType '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<UniversalCodeTypeMaster>> PostUniversalCodeTypeMaster(UniversalCodeTypeMaster model)
        {
            await _universalCodeTypeService.CreateUniversalCodeType(model);
            return Ok($"UniversalCodeType '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUniversalCodeTypeMaster(long id)
        {
            var entity = await _universalCodeTypeService.GetUniversalCodeTypeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("UniversalCodeType not found!");
            }
            return Ok($"UniversalCodeType '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetUniversalCodeTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _universalCodeTypeService.GetUniversalCodeTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
