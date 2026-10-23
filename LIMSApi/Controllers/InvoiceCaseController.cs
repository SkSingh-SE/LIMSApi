using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceCaseController : ControllerBase
    {
        private readonly IInvoiceCaseService _InvoiceCaseService;

        public InvoiceCaseController(IInvoiceCaseService InvoiceCaseServce)
        {
            _InvoiceCaseService = InvoiceCaseServce;
        }

        [HttpPost("list")]
        public async Task<IActionResult> InvoiceCaseList(PageFilter filter)
        {
            return Ok(await _InvoiceCaseService.FetchInvoiceCaseList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<InvoiceCase>> GetInvoiceCase(long id)
        {
            var entity = await _InvoiceCaseService.GetInvoiceCaseDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutInvoiceCase(InvoiceCase model)
        {
            await _InvoiceCaseService.ModifyInvoiceCase(model);
            return Ok(new
            {
                status = "success",
                message = $"InvoiceCase updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<InvoiceCase>> PostInvoiceCase(InvoiceCase model)
        {
            await _InvoiceCaseService.CreateInvoiceCase(model);
            return Ok(new
            {
                status = "success",
                message = $"InvoiceCase created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteInvoiceCase(long id)
        {
            var entity = await _InvoiceCaseService.GetInvoiceCaseDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("InvoiceCase not found!");
            }
            await _InvoiceCaseService.RemoveInvoiceCase(id);
            return Ok(new
            {
                status = "success",
                message = $"InvoiceCase deleted successfully."
            });
        }

        // Returns every year-version for a LaboratoryTest and optional AnalysisType; the default-FY version is flagged IsCurrentFy.
        [HttpGet("by-lab-test/{laboratoryTestId}")]
        public async Task<ActionResult<InvoiceCaseByLabTestDto>> GetByLabTest(long laboratoryTestId, [FromQuery] long? analysisTypeId = null)
        {
            return Ok(await _InvoiceCaseService.GetByLabTest(laboratoryTestId, analysisTypeId));
        }

        // Upserts all year-versions for a LaboratoryTest in one transaction.
        [HttpPost("save-all")]
        public async Task<IActionResult> SaveAll(SaveAllInvoiceCaseDto dto)
        {
            await _InvoiceCaseService.SaveAllInvoiceCases(dto);
            return Ok(new
            {
                status = "success",
                message = "Invoice Case prices saved successfully."
            });
        }
    }
}
