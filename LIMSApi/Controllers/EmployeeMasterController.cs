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
            return Ok(new { message = $"Employee '{model.Name}' updated successfully" });
        }

        [HttpPost("create")]
        public async Task<ActionResult<EmployeeMaster>> PostEmployeeMaster(EmployeeMaster model)
        {
            await _employeeService.CreateEmployee(model);
            return Ok(new { message = $"Employee '{model.Name}' created successfully" });
        }
        [HttpPost("doucument/create")]
        public async Task<IActionResult> PostEmployeeDocuments(List<EmployeeDocument> model)
        {
            await _employeeService.CreateDocuments(model);
            return Ok(new { message = $"Employee Ducuments created successfully" });

        }
        [HttpPost("qualification/create")]
        public async Task<IActionResult> PostEmployeeQualifications(List<EmployeeQualification> model)
        {
            await _employeeService.CreateQualifications(model);
            return Ok(new { message = $"Employee Qualifications created successfully" });
        }

        [HttpPut("document/update")]
        public async Task<IActionResult> UpdateEmployeeDocuments([FromForm] List<EmployeeDocument> Documents)
        {
            await _employeeService.ModifyDocuments(Documents);
            return Ok(new { message = $"Employee Documents updated successfully" });
        }
        [HttpPut("qualification/update")]
        public async Task<IActionResult> UpdateEmployeeQualifications(List<EmployeeQualification> model)
        {
            await _employeeService.ModifyQualifications(model);
            return Ok(new { message = $"Employee Qualifications updated successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEmployeeMaster(long id)
        {
            var entity = await _employeeService.GetEmployeeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Employee not found!");
            }
            return Ok(new { message = $"Employee '{entity.Name}' created successfully" });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetEmployeeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _employeeService.GetEmployeeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

        // Full hierarchy (CEO / Root)
        [HttpGet("org-chart")]
        public async Task<IActionResult> GetOrgChart()
        {
            var result = await _employeeService.GetOrgChartAsync();
            return result == null ? NotFound() : Ok(result);
        }

        // Lazy-load expand (children only)
        [HttpGet("{employeeId}/children")]
        public async Task<IActionResult> GetChildren(long employeeId)
        {
            return Ok(await _employeeService.GetDirectReportsAsync(employeeId));
        }
    }
}
