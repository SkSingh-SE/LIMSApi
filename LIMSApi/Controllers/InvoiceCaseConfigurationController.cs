using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceCaseConfigurationController : ControllerBase
    {
        private readonly IInvoiceCaseConfigurationService _invoiceConfigService;

        public InvoiceCaseConfigurationController(IInvoiceCaseConfigurationService invoiceConfigService)
        {
            _invoiceConfigService = invoiceConfigService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> LabScopeList(PageFilter filter)
        {
            return Ok(await _invoiceConfigService.FetchInvoiceCaseConfigurationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<InvoiceCaseConfiguration>> GetInvoiceCaseConfiguration(long id)
        {
            var entity = await _invoiceConfigService.GetInvoiceCaseConfigurationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutInvoiceCaseConfiguration(List<InvoiceCaseConfiguration> configurations)
        {
            if(configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            await _invoiceConfigService.ModifyInvoiceCaseConfiguration(configurations);
            return Ok(new
            {
                status = "success",
                message = $"Invoice Case Configuration updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<InvoiceCaseConfiguration>> PostInvoiceCaseConfiguration(List<InvoiceCaseConfiguration> configurations)
        {
            await _invoiceConfigService.CreateInvoiceCaseConfiguration(configurations);
            return Ok(new
            {
                status = "success",
                message = $"Invoice Case Configuration created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteInvoiceCaseConfiguration(long id)
        {
            var entity = await _invoiceConfigService.GetInvoiceCaseConfigurationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("LabScope not found!");
            }
            await _invoiceConfigService.RemoveInvoiceCaseConfiguration(id);
            return Ok(new
            {
                status = "success",
                message = $"Invoice Case Configuration deleted successfully."
            });
        }
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetInvoiceCaseConfigurationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _invoiceConfigService.GetInvoiceCaseConfigurationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpPost("validate-condition")]
        public IActionResult ValidateCondition([FromBody] System.Text.Json.JsonElement requestBody)
        {
            try
            {
                string? value = requestBody.GetProperty("value").GetString();
                if (string.IsNullOrWhiteSpace(value))
                    return Ok(new { isValid = false, type = "Unknown", message = "Value cannot be empty." });

                if (Helpers.ConditionMatcher.IsBaseTier(value))
                    return Ok(new { isValid = true, type = "BaseTier" });

                if (Helpers.ConditionMatcher.IsOverride(value))
                    return Ok(new { isValid = true, type = "Override" });

                return Ok(new { isValid = false, type = "Unknown", message = "Value must be a valid condition (e.g., <=1, ==2, >3) or 'override'." });
            }
            catch (Exception ex)
            {
                return Ok(new { isValid = false, type = "Unknown", message = ex.Message });
            }
        }
    }
}
