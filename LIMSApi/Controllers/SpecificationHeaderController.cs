using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecificationHeaderController : ControllerBase
    {
        private readonly ISpecificationHeaderService _specificationHeaderService;

        public SpecificationHeaderController(ISpecificationHeaderService specificationHeaderService)
        {
            _specificationHeaderService = specificationHeaderService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SpecificationHeaderList(PageFilter filter)
        {
            return Ok(await _specificationHeaderService.FetchSpecificationHeaderList(filter));
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
            return Ok($"SpecificationHeader '{model.AliasName}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<SpecificationHeader>> PostSpecificationHeader(SpecificationHeader model)
        {
            await _specificationHeaderService.CreateSpecificationHeader(model);
            return Ok($"SpecificationHeader '{model.AliasName}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecificationHeader(long id)
        {
            var entity = await _specificationHeaderService.GetSpecificationHeaderDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SpecificationHeader not found!");
            }
            return Ok($"SpecificationHeader '{entity.AliasName}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _specificationHeaderService.GetSpecificationHeaderDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
