using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OEMMasterController : ControllerBase
    {
        private readonly IOEMService _oemService;

        public OEMMasterController(IOEMService supplierService)
        {
            _oemService = supplierService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> OEMList(PageFilter filter)
        {
            return Ok(await _oemService.FetchOEMList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<OEMMaster>> GetOEMMaster(long id)
        {
            var entity = await _oemService.GetOEMDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutOEMMaster(OEMMaster model)
        {
            await _oemService.ModifyOEM(model);
            return Ok($"OEM '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<OEMMaster>> PostOEMMaster(OEMMaster model)
        {
            await _oemService.CreateOEM(model);
            return Ok($"OEM '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOEMMaster(long id)
        {
            var entity = await _oemService.GetOEMDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("OEM not found!");
            }
            return Ok($"OEM '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetOEMDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _oemService.GetOEMDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
