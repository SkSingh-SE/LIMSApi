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
    public class ParameterCategoryMasterController : ControllerBase
    {
        private readonly IParameterCategoryService _parameterCategoryService;

        public ParameterCategoryMasterController(IParameterCategoryService parameterCategoryService)
        {
            _parameterCategoryService = parameterCategoryService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ParameterCategoryList(PageFilter filter)
        {
            return Ok(await _parameterCategoryService.FetchParameterCategoryList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<ParameterCategoryMaster>> GetParameterCategoryMaster(long id)
        {
            var entity = await _parameterCategoryService.GetParameterCategoryDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutParameterCategoryMaster(ParameterCategoryMaster model)
        {
            await _parameterCategoryService.ModifyParameterCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"ParameterCategory '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ParameterCategoryMaster>> PostParameterCategoryMaster(ParameterCategoryMaster model)
        {
            await _parameterCategoryService.CreateParameterCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"ParameterCategory '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteParameterCategoryMaster(long id)
        {
            var entity = await _parameterCategoryService.GetParameterCategoryDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ParameterCategory not found!");
            }
            await _parameterCategoryService.RemoveParameterCategory(id);
            return Ok(new
            {
                status = "success",
                message = $"ParameterCategory '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetParameterCategoryDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _parameterCategoryService.GetParameterCategoryDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
