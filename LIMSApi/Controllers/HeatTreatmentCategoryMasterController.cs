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
    public class HeatTreatmentCategoryMasterController : ControllerBase
    {
        private readonly IHeatTreatmentCategoryService _heatTreatmentCategoryService;

        public HeatTreatmentCategoryMasterController(IHeatTreatmentCategoryService heatTreatmentCategoryService)
        {
            _heatTreatmentCategoryService = heatTreatmentCategoryService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> HeatTreatmentCategoryList(PageFilter filter)
        {
            return Ok(await _heatTreatmentCategoryService.FetchHeatTreatmentCategoryList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<HeatTreatmentCategoryMaster>> GetHeatTreatmentCategoryMaster(long id)
        {
            var entity = await _heatTreatmentCategoryService.GetHeatTreatmentCategoryDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutHeatTreatmentCategoryMaster(HeatTreatmentCategoryMaster model)
        {
            await _heatTreatmentCategoryService.ModifyHeatTreatmentCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"HeatTreatmentCategory '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<HeatTreatmentCategoryMaster>> PostHeatTreatmentCategoryMaster(HeatTreatmentCategoryMaster model)
        {
            await _heatTreatmentCategoryService.CreateHeatTreatmentCategory(model);
            return Ok(new
            {
                status = "success",
                message = $"HeatTreatmentCategory '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteHeatTreatmentCategoryMaster(long id)
        {
            var entity = await _heatTreatmentCategoryService.GetHeatTreatmentCategoryDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("HeatTreatmentCategory not found!");
            }
            await _heatTreatmentCategoryService.RemoveHeatTreatmentCategory(id);
            return Ok(new
            {
                status = "success",
                message = $"HeatTreatmentCategory '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetHeatTreatmentCategoryDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _heatTreatmentCategoryService.GetHeatTreatmentCategoryDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
