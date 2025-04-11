using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalibrationAgencyMasterController : ControllerBase
    {
        private readonly ICalibrationAgencyService _oemService;

        public CalibrationAgencyMasterController(ICalibrationAgencyService supplierService)
        {
            _oemService = supplierService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CalibrationAgencyList(PageFilter filter)
        {
            return Ok(await _oemService.FetchCalibrationAgencyList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<CalibrationAgencyMaster>> GetCalibrationAgencyMaster(long id)
        {
            var entity = await _oemService.GetCalibrationAgencyDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutCalibrationAgencyMaster(CalibrationAgencyMaster model)
        {
            await _oemService.ModifyCalibrationAgency(model);
            return Ok($"CalibrationAgency '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<CalibrationAgencyMaster>> PostCalibrationAgencyMaster(CalibrationAgencyMaster model)
        {
            await _oemService.CreateCalibrationAgency(model);
            return Ok($"CalibrationAgency '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCalibrationAgencyMaster(long id)
        {
            var entity = await _oemService.GetCalibrationAgencyDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("CalibrationAgency not found!");
            }
            return Ok($"CalibrationAgency '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCalibrationAgencyDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _oemService.GetCalibrationAgencyDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
