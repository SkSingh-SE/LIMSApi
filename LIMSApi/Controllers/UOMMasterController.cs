using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UOMMasterController : ControllerBase
    {
        private readonly IUOMService _uomService;

        public UOMMasterController(IUOMService uomService)
        {
            _uomService = uomService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> UOMList(PageFilter filter)
        {
            return Ok(await _uomService.FetchUOMList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<UOMMaster>> GetUOMMaster(long id)
        {
            var entity = await _uomService.GetUOMDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutUOMMaster(UOMMaster model)
        {
            await _uomService.ModifyUOM(model);
            return Ok($"UOM '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<UOMMaster>> PostUOMMaster(UOMMaster model)
        {
            await _uomService.CreateUOM(model);
            return Ok($"UOM '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUOMMaster(long id)
        {
            var entity = await _uomService.GetUOMDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("UOM not found!");
            }
            return Ok($"UOM '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetUOMDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _uomService.GetUOMDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
