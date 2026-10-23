using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService testMethodService)
        {
            _customerService = testMethodService;
        }

        [RequirePermission(Permissions.Customer.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> CustomerList(PageFilter filter)
        {
            return Ok(await _customerService.FetchCustomerList(filter));
        }


        [RequirePermission(Permissions.Customer.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerMaster(long id)
        {
            var entity = await _customerService.GetCustomerDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.Customer.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutCustomerMaster(Customer model)
        {
            await _customerService.ModifyCustomer(model);
            return Ok(new { message = $"Customer '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.Customer.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<Customer>> PostCustomerMaster(Customer model)
        {
            await _customerService.CreateCustomer(model);
            return Ok(new { message = $"Customer '{model.Name}' created successfully" });
        }

        [RequirePermission(Permissions.Customer.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomerMaster(long id)
        {
            await _customerService.RemoveCustomer(id);
            return Ok(new { message = "Customer deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _customerService.GetCustomerDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpPatch("verify/{id}")]
        public async Task<IActionResult> VerifyCustomer(long id, bool status)
        {
            await _customerService.VerifyCustomer(id, status);
            return Ok(new { message = "Customer verified successfully." });
        }

        [RequirePermission(Permissions.Customer.Read)]
        [HttpGet("change-requests/{customerId}")]
        public async Task<IActionResult> GetChangeRequests(long customerId)
        {
            var data = await _customerService.GetChangeRequests(customerId);
            return Ok(data);
        }

        [RequirePermission(Permissions.Customer.Read)]
        [HttpGet("pending-change/{customerId}")]
        public async Task<IActionResult> GetPendingChange(long customerId)
        {
            var data = await _customerService.GetPendingChangeRequest(customerId);
            return data == null ? NoContent() : Ok(data);
        }

        [RequirePermission(Permissions.Customer.Update)]
        [HttpPost("change-request/review")]
        public async Task<IActionResult> ReviewChangeRequest(ReviewChangeRequestDto dto)
        {
            await _customerService.DirectReviewChangeRequest(dto);
            return Ok(new { message = $"Change request {dto.Action}d successfully." });
        }
    }
}
