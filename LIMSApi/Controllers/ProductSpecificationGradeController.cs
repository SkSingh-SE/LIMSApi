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
    public class ProductSpecificationGradeController : ControllerBase
    {
        private readonly IProductSpecificationGradeService _productSpecificationGradeService;

        public ProductSpecificationGradeController(IProductSpecificationGradeService productSpecificationGradeService)
        {
            _productSpecificationGradeService = productSpecificationGradeService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ProductSpecificationGradeList(PageFilter filter)
        {
            return Ok(await _productSpecificationGradeService.FetchProductSpecificationGradeList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<ProductSpecificationGrade>> GetProductSpecificationGrade(long id)
        {
            var entity = await _productSpecificationGradeService.GetProductSpecificationGradeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutProductSpecificationGrade(ProductSpecificationGrade model)
        {
            await _productSpecificationGradeService.ModifyProductSpecificationGrade(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductSpecificationGrade updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProductSpecificationGrade>> PostProductSpecificationGrade(ProductSpecificationGrade model)
        {
            await _productSpecificationGradeService.CreateProductSpecificationGrade(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductSpecificationGrade created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProductSpecificationGrade(long id)
        {
            var entity = await _productSpecificationGradeService.GetProductSpecificationGradeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ProductSpecificationGrade not found!");
            }
            await _productSpecificationGradeService.RemoveProductSpecificationGrade(id);
            return Ok(new
            {
                status = "success",
                message = $"ProductSpecificationGrade deleted successfully."
            });
        }

        [HttpGet("by-product/{productSpecId}")]
        public async Task<IActionResult> GetByProductSpecification(long productSpecId)
        {
            var data = await _productSpecificationGradeService.GetByProductSpecificationId(productSpecId);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
