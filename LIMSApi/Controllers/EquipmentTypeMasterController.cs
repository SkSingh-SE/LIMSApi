using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
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
    public class EquipmentTypeMasterController : ControllerBase
    {
        private readonly IEquipmentTypeService _equipmentTypeService;

        public EquipmentTypeMasterController(IEquipmentTypeService equipmentTypeServce)
        {
            _equipmentTypeService = equipmentTypeServce;
        }

        [RequirePermission(Permissions.EquipmentType.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> EquipmentTypeList(PageFilter filter)
        {
            return Ok(await _equipmentTypeService.FetchEquipmentTypeList(filter));
        }


        [RequirePermission(Permissions.EquipmentType.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<EquipmentTypeMaster>> GetEquipmentTypeMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentTypeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.EquipmentType.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutEquipmentTypeMaster(EquipmentTypeMaster model)
        {
            await _equipmentTypeService.ModifyEquipmentType(model);
            return Ok($"EquipmentType '{model.Name}' updated successfully.");
        }

        [RequirePermission(Permissions.EquipmentType.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<EquipmentTypeMaster>> PostEquipmentTypeMaster(EquipmentTypeMaster model)
        {
            await _equipmentTypeService.CreateEquipmentType(model);
            return Ok($"EquipmentType '{model.Name}' created successfully");
        }

        [RequirePermission(Permissions.EquipmentType.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEquipmentTypeMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentTypeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("EquipmentType not found!");
            }
            await _equipmentTypeService.RemoveEquipmentType(id);
            return Ok($"EquipmentType '{entity.Name}' deleted successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetEquipmentTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _equipmentTypeService.GetEquipmentTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
