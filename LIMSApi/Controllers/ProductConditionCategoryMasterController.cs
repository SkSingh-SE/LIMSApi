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
    public class ProductConditionCategoryMasterController : ControllerBase
    {
        private readonly IProductConditionCategoryService _productConditionCategoryService;

        public ProductConditionCategoryMasterController(IProductConditionCategoryService productConditionCategoryService)
        {
            _productConditionCategoryService = productConditionCategoryService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ProductConditionCategoryList(PageFilter filter)
        {
            return Ok(await _productConditionCategoryService.FetchProductConditionCategoryList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<ProductConditionCategoryMaster>> GetProductConditionCategoryMaster(long id)
        {
            var entity = await _productConditionCategoryService.GetProductConditionCategoryDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutProductConditionCategoryMaster(ProductConditionCategoryMaster model)
        {
            await _productConditionCategoryService.ModifyProductConditionCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductConditionCategory '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProductConditionCategoryMaster>> PostProductConditionCategoryMaster(ProductConditionCategoryMaster model)
        {
            await _productConditionCategoryService.CreateProductConditionCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductConditionCategory '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProductConditionCategoryMaster(long id)
        {
            var entity = await _productConditionCategoryService.GetProductConditionCategoryDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ProductConditionCategory not found!");
            }
            await _productConditionCategoryService.RemoveProductConditionCategory(id);
            return Ok(new
            {
                status = "success",
                message = $"ProductConditionCategory '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetProductConditionCategoryDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _productConditionCategoryService.GetProductConditionCategoryDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
