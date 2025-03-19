using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecimenTypeMasterController : ControllerBase
    {
        private readonly ISpecimenTypeService _specimenService;

        public SpecimenTypeMasterController(ISpecimenTypeService specimenRepo)
        {
            _specimenService = specimenRepo;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SpecimenTypeList(PageFilter filter)
        {
            return Ok(await _specimenService.FetchSpecimenTypeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecimenTypeMaster>> GetSpecimenTypeMaster(long id)
        {
            var entity = await _specimenService.GetSpecimenTypeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSpecimenTypeMaster(SpecimenTypeMaster model)
        {
            await _specimenService.ModifySpecimenType(model);
            return Ok($"SpecimenType '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<SpecimenTypeMaster>> PostSpecimenTypeMaster(SpecimenTypeMaster model)
        {
            await _specimenService.CreateSpecimenType(model);
            return Ok($"SpecimenType '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecimenTypeMaster(long id)
        {
            var entity = await _specimenService.GetSpecimenTypeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SpecimenType not found!");
            }
            return Ok($"SpecimenType '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecimenTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _specimenService.GetSpecimenTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
