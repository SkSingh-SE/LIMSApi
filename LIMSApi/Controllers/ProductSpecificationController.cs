using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductSpecificationController : ControllerBase
    {
        private readonly IProductSpecificationService _ProductSpecificationService;

        public ProductSpecificationController(IProductSpecificationService ProductSpecificationService)
        {
            _ProductSpecificationService = ProductSpecificationService;
        }

        [RequirePermission(Permissions.ProductSpecification.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> ProductSpecificationList(PageFilter filter)
        {
            return Ok(await _ProductSpecificationService.FetchProductSpecificationList(filter));
        }
        [HttpPost("customList")]
        public async Task<IActionResult> CustomProductSpecificationList(PageFilter filter)
        {
            return Ok(await _ProductSpecificationService.FetchCustomProductSpecificationList(filter));
        }


        [RequirePermission(Permissions.ProductSpecification.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<ProductSpecification>> GetProductSpecification(long id)
        {
            var entity = await _ProductSpecificationService.GetProductSpecificationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.ProductSpecification.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutProductSpecification(ProductSpecification model)
        {
            await _ProductSpecificationService.ModifyProductSpecification(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductSpecification '{model.SpecificationName}' updated successfully."
            });
           
        }

        [RequirePermission(Permissions.ProductSpecification.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<ProductSpecification>> PostProductSpecification(ProductSpecification model)
        {
            await _ProductSpecificationService.CreateProductSpecification(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductSpecification '{model.SpecificationName}' created successfully."
            });
        }

        [RequirePermission(Permissions.ProductSpecification.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProductSpecification(long id)
        {
            var entity = await _ProductSpecificationService.GetProductSpecificationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ProductSpecification not found!");
            }
            await _ProductSpecificationService.RemoveProductSpecification(id);
            return Ok(new
            {
                status = "success",
                message = $"ProductSpecification '{entity.SpecificationName}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetProductSpecificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _ProductSpecificationService.GetProductSpecificationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
