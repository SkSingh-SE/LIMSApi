using LIMSApi.Dtos;
using LIMSApi.Helpers;
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
        private readonly IFileUploadService _fileUploadService;

        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif"
        };

        public EmployeeMasterController(IEmployeeService employeeService, IFileUploadService fileUploadService)
        {
            _employeeService = employeeService;
            _fileUploadService = fileUploadService;
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
            await _employeeService.RemoveEmployee(id);
            return Ok(new { message = $"Employee '{entity.Name}' deleted successfully" });
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

        [HttpPost("upload-profile-image")]
        [RequestSizeLimit(FileUploadValidator.MaxFileSizeBytes)]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            if (loggedInUser == null || loggedInUser.EmployeeID == 0)
                throw new UnauthorizedAccessException("User is not associated with an employee.");

            // Validate file is an image
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedImageExtensions.Contains(extension))
                return BadRequest(new { success = false, message = $"Only image files are allowed. Supported formats: {string.Join(", ", AllowedImageExtensions)}" });

            // Full file validation (size, MIME, magic bytes)
            var validationError = await FileUploadValidator.ValidateAsync(file);
            if (validationError != null)
                return BadRequest(new { success = false, message = validationError });

            // Upload file to /Uploads/Employee/
            var uploadedFile = await _fileUploadService.UploadFileAsync(file, FileType.Employee, null, $"profile_{loggedInUser.EmployeeID}");

            // Update employee's ProfileImagePath
            await _employeeService.UpdateProfileImage(loggedInUser.EmployeeID, uploadedFile.FilePath);

            return Ok(new { message = "Profile image uploaded successfully", filePath = uploadedFile.FilePath });
        }

        [HttpGet("profile-image")]
        public async Task<IActionResult> GetProfileImage()
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            if (loggedInUser == null || loggedInUser.EmployeeID == 0)
                throw new UnauthorizedAccessException("User is not associated with an employee.");

            var employee = await _employeeService.GetEmployeeDetails(loggedInUser.EmployeeID);
            return Ok(new { profileImagePath = employee.ProfileImagePath });
        }
    }
}
