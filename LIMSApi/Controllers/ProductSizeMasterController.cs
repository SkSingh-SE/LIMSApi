using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizeMasterController : ControllerBase
    {
        private readonly IProductSizeMasterService _service;

        public ProductSizeMasterController(IProductSizeMasterService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.ProductSizeMaster.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> ProductSizeList(PageFilter filter)
        {
            return Ok(await _service.FetchProductSizeList(filter));
        }

        [RequirePermission(Permissions.ProductSizeMaster.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<ProductSizeMaster>> GetProductSize(long id)
        {
            var entity = await _service.GetProductSizeDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.ProductSizeMaster.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutProductSize(ProductSizeMaster model)
        {
            await _service.ModifyProductSize(model);
            return Ok(new { status = "success", message = $"Product Size '{model.DisplayName}' updated successfully." });
        }

        [RequirePermission(Permissions.ProductSizeMaster.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<ProductSizeMaster>> PostProductSize(ProductSizeMaster model)
        {
            await _service.CreateProductSize(model);
            return Ok(new { status = "success", message = $"Product Size '{model.DisplayName}' created successfully." });
        }

        [RequirePermission(Permissions.ProductSizeMaster.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProductSize(long id)
        {
            var entity = await _service.GetProductSizeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Product Size not found!");
            }
            await _service.RemoveProductSize(id);
            return Ok(new { status = "success", message = $"Product Size '{entity.DisplayName}' deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetProductSizeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _service.GetProductSizeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
