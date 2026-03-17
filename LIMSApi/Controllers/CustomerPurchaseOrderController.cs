using LIMSApi.Dtos;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/customer-po")]
    [ApiController]
    public class CustomerPurchaseOrderController : ControllerBase
    {
        private readonly ICustomerPurchaseOrderService _service;

        public CustomerPurchaseOrderController(ICustomerPurchaseOrderService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAll([FromBody] PageFilter filter)
        {
            var result = await _service.GetAll(filter);
            return Ok(result);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetById(id);
            if (result == null)
                return NotFound(new { message = "Purchase Order not found" });
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCustomerPurchaseOrderDto dto)
        {
            var result = await _service.Create(dto);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CreateCustomerPurchaseOrderDto dto)
        {
            var result = await _service.Update(id, dto);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.Delete(id);
            return Ok(new { message = "Purchase Order deleted successfully" });
        }

        [HttpGet("dropdown/{customerId}")]
        public async Task<IActionResult> GetDropdown(long customerId, [FromQuery] string? searchTerm, [FromQuery] int pageNo = 0, [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetDropdown(customerId, searchTerm, pageNo, pageSize);
            return Ok(result);
        }

        [HttpGet("utilization/{id}")]
        public async Task<IActionResult> GetUtilization(long id)
        {
            var result = await _service.GetUtilization(id);
            if (result == null)
                return NotFound(new { message = "Purchase Order not found" });
            return Ok(result);
        }
    }
}
