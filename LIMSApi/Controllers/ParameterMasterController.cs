using LIMSApi.Dtos;
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
        public async Task<IActionResult> ChemicalParameterList(PageFilter filter)
        {
            return Ok(await _parameterService.FetchChemicalParameterList(filter));
        }
        [HttpPost("mechanical-list")]
        public async Task<IActionResult> MechanicalParameterList(PageFilter filter)
        {
            return Ok(await _parameterService.FetchMechanicalParameterList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<ParameterMaster>> GetParameterMaster(long id)
        {
            var entity = await _parameterService.GetParameterDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
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
        public async Task<IActionResult> DeleteParameterMaster(long id)
        {
            var entity = await _parameterService.GetParameterDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Parameter not found!");
            }
            await _parameterService.RemoveParameter(id);
            return Ok(new
            {
                status = "success",
                message = $"Parameter '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _parameterService.GetParameterDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
