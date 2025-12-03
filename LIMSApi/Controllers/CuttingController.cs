using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuttingController : ControllerBase
    {
        private readonly ICuttingService _service;

        public CuttingController(ICuttingService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CuttingChargeHeader model)
        {
             await _service.CreateAsync(model);
            return Ok(new
            {
                status = "success",
                message = $"Cutting Charge created successfully."  
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAll(PageFilter filter)
        {
            return Ok(await _service.GetAllAsync(filter));
        }

        [HttpGet("details/{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet("by-inward/{inwardId:long}")]
        public async Task<IActionResult> GetByInward(long inwardId)
        {
            return Ok(await _service.GetByInwardIdAsync(inwardId));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(CuttingChargeHeader model)
        {
            await _service.UpdateAsync(model);
            return Ok( new
            {
                status = "success",
                message = $"Cutting Charge updated successfully."
            });
        }
    }
}
