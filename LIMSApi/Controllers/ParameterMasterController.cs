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

        [HttpGet("details/{id}")]
        [RequirePermission(Permissions.Parameter.ReadChemical)]
        public async Task<ActionResult<ParameterMaster>> GetParameterMaster(long id)
        {
            var entity = await _parameterService.GetParameterDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        [RequirePermission(Permissions.Parameter.Update)]
        public async Task<IActionResult> PutParameterMaster(ParameterMaster model)
        {
            await _parameterService.ModifyParameter(model);
            return Ok(new
            {
                status = "success",
                message = $"Parameter '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        [RequirePermission(Permissions.Parameter.Create)]
        public async Task<ActionResult<ParameterMaster>> PostParameterMaster(ParameterMaster model)
        {
            await _parameterService.CreateParameter(model);
            return Ok(new
            {
                status = "success",
                message = $"Parameter '{model.Name}' created successfully."
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

