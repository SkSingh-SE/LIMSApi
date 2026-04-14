using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierMasterController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierMasterController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [RequirePermission(Permissions.Supplier.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> SupplierList(PageFilter filter)
        {
            return Ok(await _supplierService.FetchSupplierList(filter));
        }


        [RequirePermission(Permissions.Supplier.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<SupplierMaster>> GetSupplierMaster(long id)
        {
            var entity = await _supplierService.GetSupplierDetails(id);

            return entity == null ? NotFound("Supplier not found!") : Ok(entity);
        }


        [RequirePermission(Permissions.Supplier.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutSupplierMaster(SupplierMaster model)
        {
            await _supplierService.ModifySupplier(model);
            return Ok(new
            {
                status = "success",
                message = $"Supplier '{model.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.Supplier.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<SupplierMaster>> PostSupplierMaster([FromForm] SupplierMaster model)
        {
            await _supplierService.CreateSupplier(model);
            return Ok(new
            {
                status = "success",
                message = $"Supplier '{model.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.Supplier.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSupplierMaster(long id)
        {
            var entity = await _supplierService.GetSupplierDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Supplier not found!");
            }
            await _supplierService.RemoveSupplier(id);
            return Ok(new
            {
                status = "success",
                message = $"Supplier '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSupplierDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _supplierService.GetSupplierDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
