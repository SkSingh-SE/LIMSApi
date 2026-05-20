using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachiningChargeMasterController : ControllerBase
    {
        private readonly IMachiningChargeMasterService _service;

        public MachiningChargeMasterController(IMachiningChargeMasterService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List(PageFilter filter)
        {
            return Ok(await _service.FetchMachiningChargeMasterList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var entity = await _service.GetMachiningChargeMasterDetails(id);
            return Ok(entity);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] MachiningChargeMaster model)
        {
            await _service.CreateMachiningChargeMaster(model);
            return Ok(new { message = $"Machining Charge Master '{model.SpecimenSize}' created successfully." });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] MachiningChargeMaster model)
        {
            await _service.ModifyMachiningChargeMaster(model);
            return Ok(new { message = $"Machining Charge Master '{model.SpecimenSize}' updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _service.GetMachiningChargeMasterDetails(id);
            await _service.RemoveMachiningChargeMaster(id);
            return Ok(new { message = $"Machining Charge Master '{entity.SpecimenSize}' deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            var data = await _service.GetMachiningChargeMasterDropdown(searchTerm, pageNo, pageSize);
            return Ok(data);
        }

        [HttpGet("by-test")]
        public async Task<IActionResult> GetByLabTestAndStandard([FromQuery] long labTestId, [FromQuery] long standardId)
        {
            var data = await _service.GetByLabTestAndStandard(labTestId, standardId);
            return Ok(data);
        }
    }
}
