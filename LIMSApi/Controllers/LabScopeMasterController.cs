using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabScopeMasterController : ControllerBase
    {
        private readonly ILabScopeService _labScopeService;

        public LabScopeMasterController(ILabScopeService labScopeServce)
        {
            _labScopeService = labScopeServce;
        }

        [HttpPost("list")]
        public async Task<IActionResult> LabScopeList(PageFilter filter)
        {
            return Ok(await _labScopeService.FetchLabScopeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<LabScopeMaster>> GetLabScopeMaster(long id)
        {
            var entity = await _labScopeService.GetLabScopeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutLabScopeMaster(LabScopeMaster model)
        {
            await _labScopeService.ModifyLabScope(model);
            return Ok($"LabScope '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<LabScopeMaster>> PostLabScopeMaster(LabScopeMaster model)
        {
            await _labScopeService.CreateLabScope(model);
            return Ok($"LabScope '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLabScopeMaster(long id)
        {
            var entity = await _labScopeService.GetLabScopeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("LabScope not found!");
            }
            return Ok($"LabScope '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetLabScopeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _labScopeService.GetLabScopeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
