using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentTypeMasterController : ControllerBase
    {
        private readonly IEquipmentTypeService _equipmentTypeService;

        public EquipmentTypeMasterController(IEquipmentTypeService equipmentTypeServce)
        {
            _equipmentTypeService = equipmentTypeServce;
        }

        [HttpPost("list")]
        public async Task<IActionResult> EquipmentTypeList(PageFilter filter)
        {
            return Ok(await _equipmentTypeService.FetchEquipmentTypeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<EquipmentTypeMaster>> GetEquipmentTypeMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentTypeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutEquipmentTypeMaster(EquipmentTypeMaster model)
        {
            await _equipmentTypeService.ModifyEquipmentType(model);
            return Ok($"EquipmentType '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<EquipmentTypeMaster>> PostEquipmentTypeMaster(EquipmentTypeMaster model)
        {
            await _equipmentTypeService.CreateEquipmentType(model);
            return Ok($"EquipmentType '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEquipmentTypeMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentTypeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("EquipmentType not found!");
            }
            return Ok($"EquipmentType '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetEquipmentTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _equipmentTypeService.GetEquipmentTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
