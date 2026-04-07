using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;

namespace LIMSApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _service;

        public SettingsController(ISettingsService service)
        {
            _service = service;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(long organizationId = 0, CancellationToken cancellationToken = default)
        {
            var dto = await _service.GetAllDtoAsync(organizationId, cancellationToken);
            return Ok(dto);
        }

        [HttpPost("save-organization")]
        public async Task<IActionResult> SaveOrganization( OrganizationDto orgDto, CancellationToken cancellationToken = default)
        {
            var saved = await _service.SaveOrganizationAsync(orgDto, cancellationToken);
            return Ok(saved);
        }

        [HttpPost("save-nabl")]
        public async Task<IActionResult> SaveNabl( NablDto nablDto, CancellationToken cancellationToken = default)
        {
            var saved = await _service.SaveNablAsync(nablDto, cancellationToken);
            return Ok(saved);
        }

        [HttpPost("save-numbering")]
        public async Task<IActionResult> SaveNumbering(NumberingDto numberingDto, CancellationToken cancellationToken = default)
        {
            var result = await _service.SaveNumberingAsync(numberingDto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("save-gst")]
        public async Task<IActionResult> SaveGst(GstDto gstDto, CancellationToken cancellationToken = default)
        {
            var saved = await _service.SaveGstAsync(gstDto, cancellationToken);
            return Ok(saved);
        }

        [HttpPost("save-financial-year")]
        public async Task<IActionResult> SaveFinancialYear(FinancialYearDto yearDto, CancellationToken cancellationToken = default)
        {
            var saved = await _service.SaveFinancialYearAsync(yearDto, cancellationToken);
            return Ok(saved);
        }

        [HttpPost("save-signatories")]
        public async Task<IActionResult> SaveSignatories(
            SaveSignatoriesRequest request, CancellationToken cancellationToken = default)
        {
            var saved = await _service.SaveSignatoriesAsync(request.Signatories.ToArray(), cancellationToken);
            return Ok(saved);
        }

        [HttpPost("save-all")]
        public async Task<IActionResult> SaveAll(SaveAllRequest request, CancellationToken cancellationToken = default)
        {
            await _service.SaveAllAsync(request.Payload, cancellationToken);
            return Ok();
        }

        [HttpPost("upload-organization-logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadOrganizationLogo(IFormFile logo, CancellationToken cancellationToken = default)
        {
            var path = await _service.UploadOrganizationLogoAsync(logo, cancellationToken);
            return Ok(new { url = path });
        }

        [HttpPost("upload-nabl-certificate")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadNablCertificate(IFormFile logo, CancellationToken cancellationToken = default)
        {
            var path = await _service.UploadNablCertificateAsync(logo, cancellationToken);
            return Ok(new { url = path });
        }

        [HttpPost("upload-signature")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSignature(IFormFile logo, CancellationToken cancellationToken = default)
        {
            var path = await _service.UploadSignatureAsync(logo, cancellationToken);
            return Ok(new { url = path });
        }

        [HttpDelete("signatory/{id}")]
        public async Task<IActionResult> DeleteSignatory(long id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteSignatoryAsync(id, cancellationToken);
            return Ok();
        }

        // =====================
        // Financial Year Management
        // =====================

        [HttpGet("financial-years")]
        public async Task<IActionResult> GetAllFinancialYears(CancellationToken cancellationToken = default)
        {
            var list = await _service.GetAllFinancialYearsAsync(cancellationToken);
            return Ok(list);
        }

        [HttpGet("financial-years/dropdown")]
        public async Task<IActionResult> GetFinancialYearsDropdown(CancellationToken cancellationToken = default)
        {
            var list = await _service.GetFinancialYearsDropdownAsync(cancellationToken);
            return Ok(list);
        }

        [HttpPut("financial-years/{id}/set-default")]
        public async Task<IActionResult> SetDefaultFinancialYear(long id, CancellationToken cancellationToken = default)
        {
            await _service.SetDefaultFinancialYearAsync(id, cancellationToken);
            return Ok(new { message = "Default Financial Year updated successfully." });
        }

        [HttpDelete("financial-years/{id}")]
        public async Task<IActionResult> DeleteFinancialYear(long id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteFinancialYearAsync(id, cancellationToken);
            return Ok(new { message = "Financial Year deleted successfully." });
        }
    }

}
