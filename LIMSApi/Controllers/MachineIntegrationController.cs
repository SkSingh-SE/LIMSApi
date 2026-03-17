using LIMSApi.Dtos;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/machine-integration")]
    [ApiController]
    public class MachineIntegrationController : ControllerBase
    {
        private readonly IMachineIntegrationService _service;

        public MachineIntegrationController(IMachineIntegrationService service)
        {
            _service = service;
        }

        [HttpPost("push-data")]
        public async Task<IActionResult> PushData([FromBody] MachineDataDto dto)
        {
            var result = await _service.ProcessMachineData(dto);
            return Ok(result);
        }

        [HttpGet("pending-tests/{equipmentId}")]
        public async Task<IActionResult> GetPendingTests(long equipmentId)
        {
            var result = await _service.GetPendingTestsForEquipment(equipmentId);
            return Ok(result);
        }

        [HttpGet("logs/{headerId}")]
        public async Task<IActionResult> GetLogs(long headerId)
        {
            var result = await _service.GetMachineDataLogs(headerId);
            return Ok(result);
        }
    }
}
