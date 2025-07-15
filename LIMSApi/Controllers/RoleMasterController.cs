using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleMasterController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleMasterController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> RoleList(PageFilter filter)
        {
            return Ok(await _roleService.FetchRoleList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<RoleMaster>> GetRoleMaster(long id)
        {
            var entity = await _roleService.GetRoleDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutRoleMaster(RoleMaster model)
        {
            await _roleService.ModifyRole(model);
            return Ok(new
            {
                status = "success",
                message = $"Role '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<RoleMaster>> PostRoleMaster(RoleMaster model)
        {
            await _roleService.CreateRole(model);
            return Ok(new
            {
                status = "success",
                message = $"Role '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRoleMaster(long id)
        {
            var entity = await _roleService.GetRoleDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Role not found!");
            }
            await _roleService.RemoveRole(id);
            return Ok(new
            {
                status = "success",
                message = $"Role '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetRoleDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _roleService.GetRoleDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
