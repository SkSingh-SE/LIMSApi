using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandardOrganizationMasterController : ControllerBase
    {
        private readonly IStandardOrganizationService _standardOrganizationService;

        public StandardOrganizationMasterController(IStandardOrganizationService organizationService)
        {
            _standardOrganizationService = organizationService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> StandardOrganizationList(PageFilter filter)
        {
            return Ok(await _standardOrganizationService.FetchStandardOrganizationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<StandardOrganizationMaster>> GetStandardOrganizationMaster(long id)
        {
            var entity = await _standardOrganizationService.GetStandardOrganizationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutStandardOrganizationMaster(StandardOrganizationMaster model)
        {
            await _standardOrganizationService.ModifyStandardOrganization(model);
            return Ok($"StandardOrganization '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<StandardOrganizationMaster>> PostStandardOrganizationMaster(StandardOrganizationMaster model)
        {
            await _standardOrganizationService.CreateStandardOrganization(model);
            return Ok($"StandardOrganization '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStandardOrganizationMaster(long id)
        {
            var entity = await _standardOrganizationService.GetStandardOrganizationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("StandardOrganization not found!");
            }
            return Ok($"StandardOrganization '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetStandardOrganizationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _standardOrganizationService.GetStandardOrganizationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
