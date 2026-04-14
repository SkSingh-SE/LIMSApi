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
    public class CalibrationAgencyMasterController : ControllerBase
    {
        private readonly ICalibrationAgencyService _calibrationService;

        public CalibrationAgencyMasterController(ICalibrationAgencyService calibrationService)
        {
            _calibrationService = calibrationService;
        }

        [RequirePermission(Permissions.CalibrationAgency.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> CalibrationAgencyList(PageFilter filter)
        {
            return Ok(await _calibrationService.FetchCalibrationAgencyList(filter));
        }


        [RequirePermission(Permissions.CalibrationAgency.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<CalibrationAgencyMaster>> GetCalibrationAgencyMaster(long id)
        {
            var entity = await _calibrationService.GetCalibrationAgencyDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.CalibrationAgency.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutCalibrationAgencyMaster(CalibrationAgencyMaster model)
        {
            await _calibrationService.ModifyCalibrationAgency(model);
            return Ok(new
            {
                status = "success",
                message = $"CalibrationAgency '{model.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.CalibrationAgency.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<CalibrationAgencyMaster>> PostCalibrationAgencyMaster(CalibrationAgencyMaster model)
        {
            await _calibrationService.CreateCalibrationAgency(model);
            return Ok(new
            {
                status = "success",
                message = $"CalibrationAgency '{model.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.CalibrationAgency.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCalibrationAgencyMaster(long id)
        {
            var entity = await _calibrationService.GetCalibrationAgencyDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("CalibrationAgency not found!");
            }
            await _calibrationService.RemoveCalibrationAgency(id);
            return Ok(new
            {
                status = "success",
                message = $"CalibrationAgency '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCalibrationAgencyDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _calibrationService.GetCalibrationAgencyDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
