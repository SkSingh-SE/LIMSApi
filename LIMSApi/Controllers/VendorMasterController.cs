using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorMasterController : ControllerBase
    {
        private readonly IVendorService _supplierService;

        public VendorMasterController(IVendorService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> VendorList(PageFilter filter)
        {
            return Ok(await _supplierService.FetchVendorList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<VendorMaster>> GetVendorMaster(long id)
        {
            var entity = await _supplierService.GetVendorDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutVendorMaster(VendorMaster model)
        {
            await _supplierService.ModifyVendor(model);
            return Ok($"Vendor '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<VendorMaster>> PostVendorMaster(VendorMaster model)
        {
            await _supplierService.CreateVendor(model);
            return Ok($"Vendor '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVendorMaster(long id)
        {
            var entity = await _supplierService.GetVendorDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Vendor not found!");
            }
            return Ok($"Vendor '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetVendorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _supplierService.GetVendorDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
