using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialSpecificationController : ControllerBase
    {
        private readonly ISpecificationHeaderService _specificationHeaderService;

        public MaterialSpecificationController(ISpecificationHeaderService specificationHeaderService)
        {
            _specificationHeaderService = specificationHeaderService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SpecificationHeaderList(PageFilter filter)
        {
            return Ok(await _specificationHeaderService.FetchSpecificationHeaderList(filter));
        }

        [HttpPost("customList")]
        public async Task<IActionResult> CustomSpecificationHeaderList(PageFilter filter)
        {
            return Ok(await _specificationHeaderService.FetchCustomSpecificationHeaderList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecificationHeader>> GetSpecificationHeader(long id)
        {
            var entity = await _specificationHeaderService.GetSpecificationHeaderDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSpecificationHeader(SpecificationHeader model)
        {
            await _specificationHeaderService.ModifySpecificationHeader(model);
            return Ok(new
            {
                status = "success",
                message = $"Specification Header '{model.AliasName}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<SpecificationHeader>> PostSpecificationHeader(SpecificationHeader model)
        {
            await _specificationHeaderService.CreateSpecificationHeader(model);
            return Ok(new
            {
                status = "success",
                message = $"Specification Header '{model.AliasName}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecificationHeader(long id)
        {
            var entity = await _specificationHeaderService.GetSpecificationHeaderDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SpecificationHeader not found!");
            }
            await _specificationHeaderService.RemoveSpecificationHeader(id);
            return Ok(new
            {
                status = "success",
                message = $"Specification Header '{entity.AliasName}' created successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _specificationHeaderService.GetSpecificationHeaderDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

        [HttpGet("grade-dropdown")]
        public async Task<IActionResult> GetGradeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _specificationHeaderService.GetGradeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("default-standard/{gradeId}")]
        public async Task<IActionResult> GetDefaultStandardForSpecification(long gradeId)
        {
            var data = await _specificationHeaderService.GetDefaultStandardForSpecification(gradeId);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("test-methods")]
        public async Task<IActionResult> GetDefaultStandardForSpecification(long gradeId1 , long gradeId2)
        {
            var data = await _specificationHeaderService.GetTestMethodsForSpecifications(gradeId1,gradeId2);
            return data == null ? NoContent() : Ok(data);
        }

    }
}
