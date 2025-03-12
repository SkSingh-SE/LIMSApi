using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationMasterController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;

        public OrganisationMasterController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> OrganisationList(PageFilter filter)
        {
            return Ok(await _organisationService.FetchOrganisationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<OrganisationMaster>> GetOrganisationMaster(long id)
        {
            var entity = await _organisationService.GetOrganisationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutOrganisationMaster(OrganisationMaster model)
        {
            await _organisationService.ModifyOrganisation(model);
            return Ok($"Organisation '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<OrganisationMaster>> PostOrganisationMaster(OrganisationMaster model)
        {
            await _organisationService.CreateOrganisation(model);
            return Ok($"Organisation '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrganisationMaster(long id)
        {
            var entity = await _organisationService.GetOrganisationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Organisation not found!");
            }
            return Ok($"Organisation '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetOrganisationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _organisationService.GetOrganisationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
