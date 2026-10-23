using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuMasterController : ControllerBase
    {
        private readonly IMenuService _MenuService;

        public MenuMasterController(IMenuService MenuService)
        {
            _MenuService = MenuService;
        }

        [RequirePermission(Permissions.Menu.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> MenuList(PageFilter filter)
        {
            return Ok(await _MenuService.FetchMenuList(filter));
        }


        [RequirePermission(Permissions.Menu.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<MenuMaster>> GetMenuMaster(long id)
        {
            var entity = await _MenuService.GetMenuDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.Menu.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutMenuMaster(MenuMaster model)
        {
            await _MenuService.ModifyMenu(model);
            return Ok(new
            {
                status = "success",
                message = $"Menu '{model.Title}' updated successfully."
            });
        }

        [RequirePermission(Permissions.Menu.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<MenuMaster>> PostMenuMaster(MenuMaster model)
        {
            await _MenuService.CreateMenu(model);
            return Ok(new
            {
                status = "success",
                message = $"Menu '{model.Title}' updated successfully."
            });
        }

        [RequirePermission(Permissions.Menu.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMenuMaster(long id)
        {
            var entity = await _MenuService.GetMenuDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Menu not found!");
            }
            await _MenuService.RemoveMenu(id);
            return Ok(new
            {
                status = "success",
                message = $"Menu '{entity.Title}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetMenuDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _MenuService.GetMenuDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("submenu-dropdown")]
        public async Task<IActionResult> GetSubMenuDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _MenuService.GetSubMenuDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpPost("update-permission")]
        [RequirePermission(Permissions.Menu.AssignPermission)]
        public async Task<IActionResult> UpdateMenuPermission(long menuId, List<PermissionMaster> updatedPermissions)
        {
            if(menuId < 1)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = $"Menu Id cannot be blank."
                });
            }
            var result = await _MenuService.UpdateMenuPermission(menuId, updatedPermissions);
            if (result)
            {
                return Ok(new
                {
                    status = "success",
                    message = $"Menu permissions updated successfully."
                });
            }
            return BadRequest(new
            {
                status = "error",
                message = $"Failed to update menu permissions."
            });
        }

        [HttpGet("get-permission/{menuId}")]
        [RequirePermission(Permissions.Menu.Read)]
        public async Task<IActionResult> GetMenuPermission(long menuId)
        {
            var data = await _MenuService.GetMenuPermissions(menuId);
            if (data == null)
            {
                return NoContent();
            }
            return Ok(data);
        }

        [HttpPost("submenu-list")]
        [RequirePermission(Permissions.Menu.Read)]
        public async Task<IActionResult> SubMenuList(PageFilter filter)
        {
            return Ok(await _MenuService.FetchSubMenuList(filter));
        }

        [RequirePermission(Permissions.Menu.Read)]
        [HttpGet("tree")]
        public async Task<IActionResult> GetMenuTree()
        {
            return Ok(await _MenuService.GetMenuTree());
        }

    }
}
