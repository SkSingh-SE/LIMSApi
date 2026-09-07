using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParameterMasterController : ControllerBase
    {
        private readonly IParameterService _parameterService;

        public ParameterMasterController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        [HttpPost("chemical-list")]
        [RequirePermission(Permissions.Parameter.ReadChemical)]
        public async Task<IActionResult> ChemicalParameterList(PageFilter filter)
        {
            return Ok(await _parameterService.FetchChemicalParameterList(filter));
        }

        [HttpPost("mechanical-list")]
        [RequirePermission(Permissions.Parameter.ReadMechanical)]
        public async Task<IActionResult> MechanicalParameterList(PageFilter filter)
        {
            // Returns Mechanical + Observation parameters
            return Ok(await _parameterService.FetchMechanicalParameterList(filter));
        }

        [HttpPost("list")]
        [RequirePermission(Permissions.Parameter.ReadChemical)]
        public async Task<IActionResult> GetAllParametersUnified([FromBody] PageFilter filter)
        {
            return Ok(await _parameterService.GetAllParametersUnified(filter));
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetParameterTypes()
        {
            return Ok(await _parameterService.GetParameterTypesMetadata());
        }

        [HttpGet("details/{id}")]
        [RequirePermission(Permissions.Parameter.ReadChemical)]
        public async Task<IActionResult> GetParameterMaster(long id)
        {
            var entity = await _parameterService.GetParameterDetailById(id);
            return Ok(entity);
        }

        [HttpPut("update")]
        [RequirePermission(Permissions.Parameter.Update)]
        public async Task<IActionResult> PutParameterMaster([FromBody] ParameterUpdateDto model)
        {
            await _parameterService.ModifyParameterUnified(model);
            return Ok(new
            {
                status = "success",
                id = model.ID,
                message = $"Parameter '{model.Name}' ({model.Code}) updated successfully."
            });
        }

        [HttpPost("create")]
        [RequirePermission(Permissions.Parameter.Create)]
        public async Task<IActionResult> PostParameterMaster([FromBody] ParameterCreateDto model)
        {
            var id = await _parameterService.CreateParameterUnified(model);
            return Ok(new
            {
                status = "success",
                id = id,
                message = $"Parameter '{model.Name}' ({model.Code}) created successfully."
            });
        }

        [HttpPost("toggle-status/{id}")]
        [RequirePermission(Permissions.Parameter.Update)]
        public async Task<IActionResult> ToggleParameterStatus(long id)
        {
            var newStatus = await _parameterService.ToggleParameterStatus(id);
            return Ok(new
            {
                status = "success",
                isActive = newStatus,
                message = $"Parameter {(newStatus ? "activated" : "deactivated")} successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        [RequirePermission(Permissions.Parameter.Delete)]
        public async Task<IActionResult> DeleteParameterMaster(long id)
        {
            var entity = await _parameterService.GetParameterDetails(id);
            if (entity == null)
                throw new InvalidOperationException("Parameter not found!");
            await _parameterService.RemoveParameter(id);
            return Ok(new
            {
                status = "success",
                message = $"Parameter '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize, [FromQuery] string? elementTypes = null)
        {
            var data = await _parameterService.GetParameterDropdown(searchTerm, pageNo, pageSize, elementTypes);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("chemical-dropdown")]
        public async Task<IActionResult> GetChemicalParameterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _parameterService.GetChemicalParameterDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        /// <summary>
        /// Dropdown for Mechanical + Observation parameters (shown in General tab).
        /// </summary>
        [HttpGet("mechanical-dropdown")]
        public async Task<IActionResult> GetMechanicalParameterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _parameterService.GetMechanicalParameterDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        /// <summary>
        /// Validates a formula expression against existing active parameter IDs.
        /// Used by the Formula Builder UI before saving.
        /// POST body: { "formula": "{P12}+({P15}/6)" }
        /// </summary>
        [HttpPost("formula/validate")]
        public async Task<IActionResult> ValidateFormula([FromBody] FormulaValidateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Formula))
                return BadRequest(new { isValid = false, error = "Formula cannot be empty." });

            var (isValid, error, paramIds) = await _parameterService.ValidateFormulaForApi(request.Formula);
            return Ok(new { isValid, error, paramIds });
        }
    }

    public class FormulaValidateRequest
    {
        public string Formula { get; set; } = string.Empty;
    }
}

