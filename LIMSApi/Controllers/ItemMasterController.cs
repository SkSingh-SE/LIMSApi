using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemMasterController : ControllerBase
    {
        private readonly IItemMasterService _testMethodService;

        public ItemMasterController(IItemMasterService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ItemMasterList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchItemList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<ItemMaster>> GetItemMaster(long id)
        {
            var entity = await _testMethodService.GetItemDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutItemMaster(ItemMaster model)
        {
            await _testMethodService.ModifyItem(model);
            return Ok($"ItemMaster '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<ItemMaster>> PostItemMaster(ItemMaster model)
        {
            await _testMethodService.CreateItem(model);
            return Ok($"ItemMaster '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteItemMaster(long id)
        {
            var entity = await _testMethodService.GetItemDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ItemMaster not found!");
            }
            return Ok($"ItemMaster '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetItemMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetItemDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
