using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpecimenOrientationCategoryMasterController : ControllerBase
    {
        private readonly ISpecimenOrientationCategoryService _specimenOrientationCategoryService;

        public SpecimenOrientationCategoryMasterController(ISpecimenOrientationCategoryService specimenOrientationCategoryService)
        {
            _specimenOrientationCategoryService = specimenOrientationCategoryService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SpecimenOrientationCategoryList(PageFilter filter)
        {
            return Ok(await _specimenOrientationCategoryService.FetchSpecimenOrientationCategoryList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecimenOrientationCategoryMaster>> GetSpecimenOrientationCategoryMaster(long id)
        {
            var entity = await _specimenOrientationCategoryService.GetSpecimenOrientationCategoryDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutSpecimenOrientationCategoryMaster(SpecimenOrientationCategoryMaster model)
        {
            await _specimenOrientationCategoryService.ModifySpecimenOrientationCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"SpecimenOrientationCategory '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<SpecimenOrientationCategoryMaster>> PostSpecimenOrientationCategoryMaster(SpecimenOrientationCategoryMaster model)
        {
            await _specimenOrientationCategoryService.CreateSpecimenOrientationCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"SpecimenOrientationCategory '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecimenOrientationCategoryMaster(long id)
        {
            var entity = await _specimenOrientationCategoryService.GetSpecimenOrientationCategoryDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SpecimenOrientationCategory not found!");
            }
            await _specimenOrientationCategoryService.RemoveSpecimenOrientationCategory(id);
            return Ok(new
            {
                status = "success",
                message = $"SpecimenOrientationCategory '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecimenOrientationCategoryDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _specimenOrientationCategoryService.GetSpecimenOrientationCategoryDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
