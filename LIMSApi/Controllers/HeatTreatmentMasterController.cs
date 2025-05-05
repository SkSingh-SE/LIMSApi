using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeatTreatmentMasterController : ControllerBase
    {
        private readonly IHeatTreatmentService _heatTreatmentService;

        public HeatTreatmentMasterController(IHeatTreatmentService heatTreatmentRepo)
        {
            _heatTreatmentService = heatTreatmentRepo;
        }

        [HttpPost("list")]
        public async Task<IActionResult> HeatTreatmentList(PageFilter filter)
        {
            return Ok(await _heatTreatmentService.FetchHeatTreatmentList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<HeatTreatmentMaster>> GetHeatTreatmentMaster(long id)
        {
            var entity = await _heatTreatmentService.GetHeatTreatmentDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutHeatTreatmentMaster(HeatTreatmentMaster model)
        {
            await _heatTreatmentService.ModifyHeatTreatment(model);
            return Ok(new
            {
                status = "success",
                message = $"HeatTreatment '{model.Name}' updated successfully."
            });
            
        }

        [HttpPost("create")]
        public async Task<ActionResult<HeatTreatmentMaster>> PostHeatTreatmentMaster(HeatTreatmentMaster model)
        {
            await _heatTreatmentService.CreateHeatTreatment(model);
            return Ok(new
            {
                status = "success",
                message = $"HeatTreatment '{model.Name}' created successfully."
            });
            
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteHeatTreatmentMaster(long id)
        {
            var entity = await _heatTreatmentService.GetHeatTreatmentDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("HeatTreatment not found!");
            }
            await _heatTreatmentService.RemoveHeatTreatment(id);
            return Ok(new
            {
                status = "success",
                message = $"HeatTreatment '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetHeatTreatmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _heatTreatmentService.GetHeatTreatmentDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
