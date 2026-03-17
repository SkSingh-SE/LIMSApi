using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceDimensionTypeController : ControllerBase
    {
        private readonly IPriceDimensionTypeService _service;

        public PriceDimensionTypeController(IPriceDimensionTypeService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List(PageFilter filter)
        {
            var result = await _service.FetchPriceDimensionTypeList(filter);
            return Ok(result);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var result = await _service.GetPriceDimensionTypeDetails(id);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(PriceDimensionType model)
        {
            await _service.CreatePriceDimensionType(model);
            return Ok(new { Success = true, Message = "Price Dimension Type created successfully!" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(PriceDimensionType model)
        {
            await _service.ModifyPriceDimensionType(model);
            return Ok(new { Success = true, Message = "Price Dimension Type updated successfully!" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.RemovePriceDimensionType(id);
            return Ok(new { Success = true, Message = "Price Dimension Type deleted successfully!" });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown([FromQuery] string? searchTerm, [FromQuery] int pageNo = 0, [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPriceDimensionTypeDropdown(searchTerm, pageNo, pageSize);
            return Ok(result);
        }
    }
}
