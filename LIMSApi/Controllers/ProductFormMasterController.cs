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
    public class ProductFormMasterController : ControllerBase
    {
        private readonly IProductFormService _productFormService;

        public ProductFormMasterController(IProductFormService productFormService)
        {
            _productFormService = productFormService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ProductFormList(PageFilter filter)
        {
            return Ok(await _productFormService.FetchProductFormList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<ProductFormMaster>> GetProductFormMaster(long id)
        {
            var entity = await _productFormService.GetProductFormDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutProductFormMaster(ProductFormMaster model)
        {
            await _productFormService.ModifyProductForm(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductForm '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProductFormMaster>> PostProductFormMaster(ProductFormMaster model)
        {
            await _productFormService.CreateProductForm(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductForm '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProductFormMaster(long id)
        {
            var entity = await _productFormService.GetProductFormDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ProductForm not found!");
            }
            await _productFormService.RemoveProductForm(id);
            return Ok(new
            {
                status = "success",
                message = $"ProductForm '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetProductFormDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _productFormService.GetProductFormDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
