using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentReferenceMaterialController : ControllerBase
    {
        private readonly IEquipmentReferenceMaterialService _service;

        public EquipmentReferenceMaterialController(IEquipmentReferenceMaterialService service)
        {
            _service = service;
        }

        [HttpGet("by-equipment/{equipmentMasterID}")]
        public async Task<IActionResult> GetByEquipment(long equipmentMasterID)
        {
            var data = await _service.GetByEquipment(equipmentMasterID);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EquipmentReferenceMaterial model)
        {
            await _service.Create(model);
            return Ok(new { message = "Reference material added successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.Delete(id);
            return Ok(new { message = "Reference material deleted successfully." });
        }
    }
}
