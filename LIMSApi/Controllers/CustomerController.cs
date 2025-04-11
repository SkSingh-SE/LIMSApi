using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService testMethodService)
        {
            _customerService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CustomerList(PageFilter filter)
        {
            return Ok(await _customerService.FetchCustomerList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerMaster(long id)
        {
            var entity = await _customerService.GetCustomerDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutCustomerMaster(Customer model)
        {
            await _customerService.ModifyCustomer(model);
            return Ok($"Customer '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<Customer>> PostCustomerMaster(Customer model)
        {
            await _customerService.CreateCustomer(model);
            return Ok($"Customer '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomerMaster(long id)
        {
            var entity = await _customerService.GetCustomerDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Customer not found!");
            }
            return Ok($"Customer '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _customerService.GetCustomerDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
