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
    public class ProductConditionMasterController : ControllerBase
    {
        private readonly IProductConditionService _productConditionService;

        public ProductConditionMasterController(IProductConditionService productConditionService)
        {
            _productConditionService = productConditionService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ProductConditionList(PageFilter filter)
        {
            return Ok(await _productConditionService.FetchProductConditionList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<ProductConditionMaster>> GetProductConditionMaster(long id)
        {
            var entity = await _productConditionService.GetProductConditionDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutProductConditionMaster(ProductConditionMaster model)
        {
            await _productConditionService.ModifyProductCondition(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductCondition '{model.Name}' updated successfully."
            });
           
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProductConditionMaster>> PostProductConditionMaster(ProductConditionMaster model)
        {
            await _productConditionService.CreateProductCondition(model);
            return Ok(new
            {
                status = "success",
                message = $"ProductCondition '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProductConditionMaster(long id)
        {
            var entity = await _productConditionService.GetProductConditionDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ProductCondition not found!");
            }
            await _productConditionService.RemoveProductCondition(id);
            return Ok(new
            {
                status = "success",
                message = $"ProductCondition '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetProductConditionDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _productConditionService.GetProductConditionDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
