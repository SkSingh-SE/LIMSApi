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
    public class ProductTestGroupController : ControllerBase
    {
        private readonly IProductTestGroupService _productTestGroupService;

        public ProductTestGroupController(IProductTestGroupService productTestGroupService)
        {
            _productTestGroupService = productTestGroupService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ProductTestGroupList(PageFilter filter)
        {
            return Ok(await _productTestGroupService.FetchProductTestGroupList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<ProductTestGroup>> GetProductTestGroup(long id)
        {
            var entity = await _productTestGroupService.GetProductTestGroupDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutProductTestGroup(ProductTestGroup model)
        {
            await _productTestGroupService.ModifyProductTestGroup(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductTestGroup updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProductTestGroup>> PostProductTestGroup(ProductTestGroup model)
        {
            await _productTestGroupService.CreateProductTestGroup(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductTestGroup created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProductTestGroup(long id)
        {
            var entity = await _productTestGroupService.GetProductTestGroupDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ProductTestGroup not found!");
            }
            await _productTestGroupService.RemoveProductTestGroup(id);
            return Ok(new
            {
                status = "success",
                message = $"ProductTestGroup deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetProductTestGroupDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _productTestGroupService.GetProductTestGroupDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("by-product/{productSpecId}")]
        public async Task<IActionResult> GetByProductSpecification(long productSpecId)
        {
            var data = await _productTestGroupService.GetByProductSpecificationId(productSpecId);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
