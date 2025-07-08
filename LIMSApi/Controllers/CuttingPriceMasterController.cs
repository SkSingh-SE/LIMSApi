using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuttingPriceMasterController : ControllerBase
    {

        private readonly ICuttingPriceMasterService _cuttingPriceService;

        public CuttingPriceMasterController(ICuttingPriceMasterService cuttingPriceService)
        {
            _cuttingPriceService = cuttingPriceService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CuttingPriceMasterList(PageFilter filter)
        {
            return Ok(await _cuttingPriceService.FetchCuttingPriceList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<CuttingPriceMaster>> GetCuttingPriceMaster(long id)
        {
            var entity = await _cuttingPriceService.GetCuttingPriceDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutCuttingPriceMaster(CuttingPriceMaster model)
        {
            await _cuttingPriceService.ModifyCuttingPrice(model);
            return Ok(new
            {
                status = "success",
                message = $"CuttingPriceMaster '{model.CuttingType}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<CuttingPriceMaster>> PostCuttingPriceMaster(CuttingPriceMaster model)
        {
            await _cuttingPriceService.CreateCuttingPrice(model);
            return Ok(new
            {
                status = "success",
                message = $"CuttingPriceMaster '{model.CuttingType}' ctreated successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCuttingPriceMaster(long id)
        {
            var entity = await _cuttingPriceService.GetCuttingPriceDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("CuttingPriceMaster not found!");
            }
            await _cuttingPriceService.RemoveCuttingPrice(id);
            return Ok(new
            {
                status = "success",
                message = $"CuttingPriceMaster '{entity.CuttingType}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCuttingPriceMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _cuttingPriceService.GetCuttingPriceDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
