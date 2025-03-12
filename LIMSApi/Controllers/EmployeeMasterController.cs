using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeMasterController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> EmployeeList(PageFilter filter)
        {
            return Ok(await _employeeService.FetchEmployeeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<EmployeeMaster>> GetEmployeeMaster(long id)
        {
            var entity = await _employeeService.GetEmployeeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutEmployeeMaster(EmployeeMaster model)
        {
            await _employeeService.ModifyEmployee(model);
            return Ok($"Employee '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<EmployeeMaster>> PostEmployeeMaster(EmployeeMaster model)
        {
            await _employeeService.CreateEmployee(model);
            return Ok($"Employee '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEmployeeMaster(long id)
        {
            var entity = await _employeeService.GetEmployeeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Employee not found!");
            }
            return Ok($"Employee '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetEmployeeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _employeeService.GetEmployeeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
