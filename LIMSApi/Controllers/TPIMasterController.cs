using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TPIMasterController : ControllerBase
    {
        private readonly ITPIMasterService _testMethodService;

        public TPIMasterController(ITPIMasterService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TPIMasterList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTPIList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TPIMaster>> GetTPIMaster(long id)
        {
            var entity = await _testMethodService.GetTPIDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTPIMaster(TPIMaster model)
        {
            await _testMethodService.ModifyTPI(model);
            return Ok($"TPIMaster '{model.AgencyName}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<TPIMaster>> PostTPIMaster(TPIMaster model)
        {
            await _testMethodService.CreateTPI(model);
            return Ok($"TPIMaster '{model.AgencyName}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTPIMaster(long id)
        {
            var entity = await _testMethodService.GetTPIDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TPIMaster not found!");
            }
            return Ok($"TPIMaster '{entity.AgencyName}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTPIMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTPIDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
