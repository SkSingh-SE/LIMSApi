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

        private readonly IIndustryMasterService _industryService;

        public IndustryMasterController(IIndustryMasterService industryService)
        {
            _industryService = industryService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> IndustryMasterList(PageFilter filter)
        {
            return Ok(await _industryService.FetchIndustryList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<IndustryMaster>> GetIndustryMaster(long id)
        {
            var entity = await _industryService.GetIndustryDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutIndustryMaster(IndustryMaster model)
        {
            await _industryService.ModifyIndustry(model);
            return Ok(new
            {
                status = "success",
                message = $"IndustryMaster '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<IndustryMaster>> PostIndustryMaster(IndustryMaster model)
        {
            await _industryService.CreateIndustry(model);
            return Ok(new
            {
                status = "success",
                message = $"IndustryMaster '{model.Name}' ctreated successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteIndustryMaster(long id)
        {
            var entity = await _industryService.GetIndustryDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("IndustryMaster not found!");
            }
            await _industryService.RemoveIndustry(id);
            return Ok(new
            {
                status = "success",
                message = $"IndustryMaster '{entity.Name}' ctreated successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetIndustryMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _industryService.GetIndustryDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
