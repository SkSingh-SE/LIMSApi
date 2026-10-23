using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LIMSApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GstValidatorController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GstValidatorController> _logger;

        public GstValidatorController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GstValidatorController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateGstin([FromBody] GstValidateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Gstin))
                return BadRequest(new { message = "GSTIN is required." });

            var apiKey = _configuration["GstZen:ApiKey"];
            var apiUrl = _configuration["GstZen:ApiUrl"] ?? "https://my.gstzen.in/api/gstin-validator/";

            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(503, new { message = "GST validation service is not configured." });

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Token", apiKey);

                var payload = JsonSerializer.Serialize(new { gstin = request.Gstin.ToUpper() });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GST validation API call failed for GSTIN: {Gstin}", request.Gstin);
                return StatusCode(500, new { message = "Failed to reach GST validation service." });
            }
        }
    }

    public class GstValidateRequest
    {
        public string Gstin { get; set; } = string.Empty;
    }
}
