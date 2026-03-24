using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly ITaxService _taxService;

        public TaxController(ITaxService taxService)
        {
            _taxService = taxService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TaxList(PageFilter filter)
        {
            return Ok(await _taxService.FetchTaxList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TaxMaster>> GetTaxMaster(long id)
        {
            var entity = await _taxService.GetTaxDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTaxMaster(TaxMaster model)
        {
            await _taxService.ModifyTax(model);
            return Ok(new { message = $"TaxMaster '{model.Name}' updated successfully." });
        }

        [HttpPost("create")]
        public async Task<ActionResult<TaxMaster>> PostTaxMaster(TaxMaster model)
        {
            await _taxService.CreateTax(model);
            return Ok(new { message = $"TaxMaster '{model.Name}' created successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTaxMaster(long id)
        {
            var entity = await _taxService.GetTaxDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TaxMaster not found!");
            }
            await _taxService.RemoveTax(id);
            return Ok(new { message = $"TaxMaster '{entity.Name}' deleted successfully" });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTaxDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _taxService.GetTaxDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
