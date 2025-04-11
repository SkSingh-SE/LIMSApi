using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustryMasterController : ControllerBase
    {

        private readonly IIndustryMasterService _testMethodService;

        public IndustryMasterController(IIndustryMasterService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> IndustryMasterList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchIndustryList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<IndustryMaster>> GetIndustryMaster(long id)
        {
            var entity = await _testMethodService.GetIndustryDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutIndustryMaster(IndustryMaster model)
        {
            await _testMethodService.ModifyIndustry(model);
            return Ok($"IndustryMaster '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<IndustryMaster>> PostIndustryMaster(IndustryMaster model)
        {
            await _testMethodService.CreateIndustry(model);
            return Ok($"IndustryMaster '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteIndustryMaster(long id)
        {
            var entity = await _testMethodService.GetIndustryDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("IndustryMaster not found!");
            }
            return Ok($"IndustryMaster '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetIndustryMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetIndustryDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
