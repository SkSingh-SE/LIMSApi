using System.Threading.Tasks;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/plan-explorer")]
    public class PlanExplorerController : ControllerBase
    {
        private readonly IPlanExplorerService _explorerService;

        public PlanExplorerController(IPlanExplorerService explorerService)
        {
            _explorerService = explorerService;
        }

        [HttpGet("product-master/{id}")]
        public async Task<IActionResult> GetProductMasterExplorer(long id)
        {
            var result = await _explorerService.GetProductMasterExplorerAsync(id);
            if (result == null) return NotFound(new { message = "Product Master not found" });
            return Ok(result);
        }

        [HttpGet("metal-classification/{id}")]
        public async Task<IActionResult> GetMetalClassificationExplorer(long id)
        {
            var result = await _explorerService.GetMetalClassificationExplorerAsync(id);
            if (result == null) return NotFound(new { message = "Metal Classification not found" });
            return Ok(result);
        }

        [HttpGet("lab-test/{id}")]
        public async Task<IActionResult> GetLabTestExplorer(long id)
        {
            var result = await _explorerService.GetLabTestExplorerAsync(id);
            if (result == null) return NotFound(new { message = "Laboratory Test not found" });
            return Ok(result);
        }

        [HttpGet("universal-search")]
        public async Task<IActionResult> UniversalTestSearch([FromQuery] string query)
        {
            var results = await _explorerService.GetUniversalTestSearchAsync(query);
            return Ok(results);
        }
    }
}
