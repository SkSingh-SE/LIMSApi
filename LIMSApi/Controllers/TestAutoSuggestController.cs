using LIMSApi.Dtos;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestAutoSuggestController : ControllerBase
    {
        private readonly ITestAutoSuggestService _service;

        public TestAutoSuggestController(ITestAutoSuggestService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get suggested tests by specification grade (via SpecificationLine → SpecificationLineLaboratoryTest).
        /// </summary>
        [HttpGet("by-specification/{gradeId}")]
        public async Task<IActionResult> GetBySpecification(long gradeId)
        {
            var results = await _service.GetSuggestedTestsBySpecification(gradeId);

            if (results == null || results.Count == 0)
                return NoContent();

            return Ok(results);
        }

        /// <summary>
        /// Get suggested tests by product specification (via ProductTestGroup).
        /// </summary>
        [HttpGet("by-product-spec/{productSpecId}")]
        public async Task<IActionResult> GetByProductSpec(long productSpecId)
        {
            var results = await _service.GetSuggestedTestsByProductSpec(productSpecId);

            if (results == null || results.Count == 0)
                return NoContent();

            return Ok(results);
        }

        /// <summary>
        /// Get unified (deduplicated) test suggestions combining specification and product test group sources.
        /// </summary>
        [HttpGet("unified")]
        public async Task<IActionResult> GetUnified([FromQuery] long? gradeId, [FromQuery] long? productSpecId)
        {
            var result = await _service.GetUnifiedSuggestions(gradeId, productSpecId);

            if (result.SuggestedTests.Count == 0)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Smart auto-suggest: scores and ranks tests based on specification, lab scope,
        /// customer history, global frequency, and trending data.
        /// </summary>
        [HttpPost("smart")]
        public async Task<IActionResult> GetSmartSuggestions([FromBody] SmartSuggestRequest request)
        {
            var result = await _service.GetSmartSuggestions(request);
            return Ok(result);
        }
    }
}
