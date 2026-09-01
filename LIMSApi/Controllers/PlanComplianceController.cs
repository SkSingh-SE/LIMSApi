using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/plan-compliance")]
    [ApiController]
    [Authorize]
    public class PlanComplianceController : ControllerBase
    {
        private readonly IPlanComplianceService _complianceService;

        public PlanComplianceController(IPlanComplianceService complianceService)
        {
            _complianceService = complianceService;
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> Evaluate([FromBody] PlanComplianceRequestDto request)
        {
            var result = await _complianceService.EvaluateComplianceAsync(request);
            return Ok(result);
        }

        [HttpGet("analysis-type-parameters/{analysisTypeId}")]
        public async Task<IActionResult> GetAnalysisTypeParameters(long analysisTypeId)
        {
            var parameters = await _complianceService.GetChemicalParametersForAnalysisTypeAsync(analysisTypeId);
            return Ok(parameters);
        }
    }
}
