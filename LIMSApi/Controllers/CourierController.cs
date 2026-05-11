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
    public class CourierController : ControllerBase
    {
        private readonly ICourierService _courierService;

        public CourierController(ICourierService remarkService)
        {
            _courierService = remarkService;
        }

        [RequirePermission(Permissions.Courier.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> CourierList(PageFilter filter)
        {
            return Ok(await _courierService.FetchCourierList(filter));
        }


        [RequirePermission(Permissions.Courier.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<CourierMaster>> GetCourierMaster(long id)
        {
            var entity = await _courierService.GetCourierDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.Courier.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutCourierMaster(CourierMaster model)
        {
            await _courierService.ModifyCourier(model);
            return Ok(new { status = "success", message = $"CourierMaster '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.Courier.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<CourierMaster>> PostCourierMaster(CourierMaster model)
        {
            await _courierService.CreateCourier(model);
            return Ok(new { status = "success", message = $"CourierMaster '{model.Name}' created successfully." });
        }

        [RequirePermission(Permissions.Courier.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourierMaster(long id)
        {
            var entity = await _courierService.GetCourierDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("CourierMaster not found!");
            }
            await _courierService.RemoveCourier(id);
            return Ok(new { status = "success", message = $"CourierMaster '{entity.Name}' deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCourierDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _courierService.GetCourierDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
