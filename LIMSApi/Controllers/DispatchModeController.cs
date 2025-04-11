using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispatchModeController : ControllerBase
    {
        private readonly IDispatchModeService _testMethodService;

        public DispatchModeController(IDispatchModeService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> DispatchModeList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchDispatchModeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<DispatchModeMaster>> GetDispatchModeMaster(long id)
        {
            var entity = await _testMethodService.GetDispatchModeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutDispatchModeMaster(DispatchModeMaster model)
        {
            await _testMethodService.ModifyDispatchMode(model);
            return Ok($"DispatchModeMaster '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<DispatchModeMaster>> PostDispatchModeMaster(DispatchModeMaster model)
        {
            await _testMethodService.CreateDispatchMode(model);
            return Ok($"DispatchModeMaster '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDispatchModeMaster(long id)
        {
            var entity = await _testMethodService.GetDispatchModeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("DispatchModeMaster not found!");
            }
            return Ok($"DispatchModeMaster '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDispatchModeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetDispatchModeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
