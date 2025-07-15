using LIMSApi.Dtos;
using LIMSApi.Models;
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

        [HttpPost("list")]
        public async Task<IActionResult> MenuList(PageFilter filter)
        {
            return Ok(await _MenuService.FetchMenuList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<MenuMaster>> GetMenuMaster(long id)
        {
            var entity = await _MenuService.GetMenuDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


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
            return data == null ? NoContent(): Ok(data);
        }

    }
}
