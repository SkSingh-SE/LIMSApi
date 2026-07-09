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
    public class MetalClassificationMasterController : ControllerBase
    {
        private readonly IMetalClassificationService _MetalClassificationService;

        public MetalClassificationMasterController(IMetalClassificationService MetalClassificationRepo)
        {
            _MetalClassificationService = MetalClassificationRepo;
        }

        [RequirePermission(Permissions.MetalClassification.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> MetalClassificationList(PageFilter filter)
        {
            return Ok(await _MetalClassificationService.FetchMetalClassificationList(filter));
        }


        [RequirePermission(Permissions.MetalClassification.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<MetalClassificationMaster>> GetMetalClassificationMaster(long id)
        {
            var entity = await _MetalClassificationService.GetMetalClassificationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.MetalClassification.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutMetalClassificationMaster(MetalClassificationMaster model)
        {
            await _MetalClassificationService.ModifyMetalClassification(model);
            return Ok(new
            {
                status = "success",
                message = $"MetalClassification '{model.Name}' updated successfully."
            });
            
        }

        [RequirePermission(Permissions.MetalClassification.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<MetalClassificationMaster>> PostMetalClassificationMaster(MetalClassificationMaster model)
        {
            await _MetalClassificationService.CreateMetalClassification(model);
            return Ok(new
            {
                status = "success",
                message = $"MetalClassification '{model.Name}' created successfully."
            });
            
        }

        [RequirePermission(Permissions.MetalClassification.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMetalClassificationMaster(long id)
        {
            var entity = await _MetalClassificationService.GetMetalClassificationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("MetalClassification not found!");
            }
            await _MetalClassificationService.RemoveMetalClassification(id);
            return Ok(new
            {
                status = "success",
                message = $"MetalClassification '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetMetalClassificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _MetalClassificationService.GetMetalClassificationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }
        
        [HttpGet("metal-parameters/{Id}")]
        public async Task<IActionResult> GeMetalParameters(long Id)
        {
            var data = await _MetalClassificationService.GetParameterByMetalId(Id);
            return data == null ? NoContent(): Ok(data);
        }

        /// Analysis techniques valid for a metal base (inherits from parent if none configured). L3 cascade.
        [HttpGet("technologies/{id}")]
        public async Task<IActionResult> GetMetalTechniques(long id)
        {
            var data = await _MetalClassificationService.GetTechniquesForMetal(id);
            return Ok(data);
        }

    }
}
