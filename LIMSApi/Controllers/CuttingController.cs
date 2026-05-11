using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
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
        [RequirePermission(Permissions.SamplePreparation.Create)]
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
        [RequirePermission(Permissions.SamplePreparation.Read)]
        public async Task<IActionResult> GetAll(PageFilter filter)
        {
            return Ok(await _service.GetAllAsync(filter));
        }

        [HttpGet("details/{id:long}")]
        [RequirePermission(Permissions.SamplePreparation.Read)]
        public async Task<IActionResult> GetById(long id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet("by-inward/{inwardId:long}")]
        [RequirePermission(Permissions.SamplePreparation.Read)]
        public async Task<IActionResult> GetByInward(long inwardId)
        {
            return Ok(await _service.GetByInwardIdAsync(inwardId));
        }

        [HttpPut("update")]
        [RequirePermission(Permissions.SamplePreparation.Update)]
        public async Task<IActionResult> Update(CuttingChargeHeader model)
        {
            await _service.UpdateAsync(model);
            return Ok( new
            {
                status = "success",
                message = $"Cutting Charge updated successfully."
            });
        }

        [HttpPut("update-status/{sampleId:long}")]
        [RequirePermission(Permissions.SamplePreparation.Update)]
        public async Task<IActionResult> UpdatePreparationStatus(long sampleId, [FromQuery] string status)
        {
            await _service.UpdatePreparationStatusAsync(sampleId, status);
            return Ok(new
            {
                status = "success",
                message = $"Preparation status updated to '{status}' successfully."
            });
        }
    }
}
