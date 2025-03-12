using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecimenOrientationMasterController : ControllerBase
    {
        private readonly ISpecimenOrientationService _specimenService;

        public SpecimenOrientationMasterController(ISpecimenOrientationService specimenRepo)
        {
            _specimenService = specimenRepo;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SpecimenOrientationList(PageFilter filter)
        {
            return Ok(await _specimenService.FetchSpecimenOrientationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecimenOrientationMaster>> GetSpecimenOrientationMaster(long id)
        {
            var entity = await _specimenService.GetSpecimenOrientationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSpecimenOrientationMaster(SpecimenOrientationMaster model)
        {
            await _specimenService.ModifySpecimenOrientation(model);
            return Ok($"SpecimenOrientation '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<SpecimenOrientationMaster>> PostSpecimenOrientationMaster(SpecimenOrientationMaster model)
        {
            await _specimenService.CreateSpecimenOrientation(model);
            return Ok($"SpecimenOrientation '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecimenOrientationMaster(long id)
        {
            var entity = await _specimenService.GetSpecimenOrientationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SpecimenOrientation not found!");
            }
            return Ok($"SpecimenOrientation '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecimenOrientationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _specimenService.GetSpecimenOrientationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
