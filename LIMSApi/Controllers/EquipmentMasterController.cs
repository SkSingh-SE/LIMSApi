using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentMasterController : ControllerBase
    {
        private readonly IEquipmentService _equipmentTypeService;

        public EquipmentMasterController(IEquipmentService equipmentTypeServce)
        {
            _equipmentTypeService = equipmentTypeServce;
        }

        [RequirePermission(Permissions.Equipment.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> EquipmentList(PageFilter filter)
        {
            return Ok(await _equipmentTypeService.FetchEquipmentList(filter));
        }


        [RequirePermission(Permissions.Equipment.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<EquipmentMaster>> GetEquipmentMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.Equipment.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutEquipmentMaster(EquipmentMaster model)
        {
            await _equipmentTypeService.ModifyEquipment(model);
            return Ok(new
            {
                status = "success",
                message = $"Equipment '{model.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.Equipment.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<EquipmentMaster>> PostEquipmentMaster(EquipmentMaster model)
        {
            await _equipmentTypeService.CreateEquipment(model);
            return Ok(new
            {
                status = "success",
                message = $"Equipment '{model.Name}' created successfully."
            });
        }
        [HttpPatch("calibration/{id}/review")]
        public async Task<IActionResult> ReviewCalibration(long id)
        {
            await _equipmentTypeService.ReviewCalibration(id);
            return Ok(new { status = "success", message = "Calibration marked as reviewed." });
        }

        [HttpPost("add-calibration")]
        public async Task<IActionResult> AddCalibration([FromForm] EquipmentCalibration model)
        {
            await _equipmentTypeService.AddEquipmentCalibration(model);
            return Ok(new
            {
                status = "success",
                message = $"Calibration added successfully."
            });
        }
        [HttpPost("add-maintenance")]
        public async Task<IActionResult> AddMaintenance([FromForm] EquipmentMaintenance model)
        {
            await _equipmentTypeService.AddEquipmentMaintenance(model);
            return Ok(new
            {
                status = "success",
                message = $"Maintenance added successfully."
            });
        }
        
        [HttpPost("add-sop")]
        public async Task<IActionResult> AddSOP([FromForm] EquipmentSOP model)
        {
            await _equipmentTypeService.AddEquipmentSOP(model);
            return Ok(new
            {
                status = "success",
                message = $"Calibration added successfully."
            });
        }

        [RequirePermission(Permissions.Equipment.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEquipmentMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Equipment not found!");
            }
            await _equipmentTypeService.RemoveEquipment(id);
            return Ok(new
            {
                status = "success",
                message = $"Equipment {entity.Name} deleted successfully."
            });
        }

        [HttpPost("delete-sop")]
        public async Task<IActionResult> DeleteEquipmentSOP(EquipmentSOP model)
        {
            await _equipmentTypeService.RemoveEquipmentSOP(model);
            return Ok(new
            {
                status = "success",
                message = $"SOP deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetEquipmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _equipmentTypeService.GetEquipmentDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
