using LIMSApi.Dtos;
using LIMSApi.Models;
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

        [HttpPost("list")]
        public async Task<IActionResult> EquipmentList(PageFilter filter)
        {
            return Ok(await _equipmentTypeService.FetchEquipmentList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<EquipmentMaster>> GetEquipmentMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutEquipmentMaster(EquipmentMaster model)
        {
            await _equipmentTypeService.ModifyEquipment(model);
            return Ok($"Equipment '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<EquipmentMaster>> PostEquipmentMaster(EquipmentMaster model)
        {
            await _equipmentTypeService.CreateEquipment(model);
            return Ok($"Equipment '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEquipmentMaster(long id)
        {
            var entity = await _equipmentTypeService.GetEquipmentDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Equipment not found!");
            }
            return Ok($"Equipment '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetEquipmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _equipmentTypeService.GetEquipmentDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
