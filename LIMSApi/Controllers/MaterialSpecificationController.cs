using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialSpecificationController : ControllerBase
    {
        private readonly ISpecificationHeaderService _service;
        private readonly LIMSContext _context;

        public MaterialSpecificationController(ISpecificationHeaderService specificationHeaderService, LIMSContext context)
        {
            _service = specificationHeaderService;
            _context = context;
        }

        [RequirePermission(Permissions.MaterialSpecification.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> SpecificationHeaderList(PageFilter filter)
        {
            return Ok(await _service.FetchSpecificationHeaderList(filter));
        }

        [HttpPost("customList")]
        public async Task<IActionResult> CustomSpecificationHeaderList(PageFilter filter)
        {
            return Ok(await _service.FetchCustomSpecificationHeaderList(filter));
        }


        [RequirePermission(Permissions.MaterialSpecification.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecificationHeader>> GetSpecificationHeader(long id)
        {
            var entity = await _service.GetSpecificationHeaderDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.MaterialSpecification.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutSpecificationHeader(SpecificationHeader model)
        {
            await _service.ModifySpecificationHeader(model);
            return Ok(new
            {
                status = "success",
                message = $"Specification Header '{model.AliasName}' updated successfully."
            });
        }

        [RequirePermission(Permissions.MaterialSpecification.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<SpecificationHeader>> PostSpecificationHeader(SpecificationHeader model)
        {
            await _service.CreateSpecificationHeader(model);
            return Ok(new
            {
                status = "success",
                message = $"Specification Header '{model.AliasName}' created successfully.",
                id = model.ID
            });
        }

        [RequirePermission(Permissions.MaterialSpecification.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecificationHeader(long id)
        {
            var entity = await _service.GetSpecificationHeaderDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SpecificationHeader not found!");
            }
            await _service.RemoveSpecificationHeader(id);
            return Ok(new
            {
                status = "success",
                message = $"Specification Header '{entity.AliasName}' created successfully."
            });
        }

        // Clone-as-new-version: returns a detached copy (IDs zeroed, Version cleared) to pre-fill the create form.
        [RequirePermission(Permissions.MaterialSpecification.Create)]
        [HttpGet("clone-template/{id}")]
        public async Task<ActionResult<SpecificationHeader>> GetCloneTemplate(long id)
        {
            var entity = await _service.GetCloneTemplate(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _service.GetSpecificationHeaderDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

        [HttpGet("grade-dropdown")]
        public async Task<IActionResult> GetGradeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _service.GetGradeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
        [HttpGet("grade-dropdown-metal-wise")]
        public async Task<IActionResult> GetGradeDropdownMetalWise( string? searchTerm, int pageNo, int pageSize, long metalId)
        {
            var data = await _service.GetGradeDropdownMetalWise(searchTerm, pageNo, pageSize,metalId);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("default-standard/{gradeId}")]
        public async Task<IActionResult> GetDefaultStandardForSpecification(long gradeId)
        {
            var data = await _service.GetDefaultStandardForSpecification(gradeId);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("test-methods")]
        public async Task<IActionResult> GetDefaultStandardForSpecification(long gradeId1 , long gradeId2)
        {
            var data = await _service.GetTestMethodsForSpecifications(gradeId1,gradeId2);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("GetChemicalElementsBySpecifications")]
        public async Task<IActionResult> GetChemicalElementsBySpecifications(long spec1Id = 0, long spec2Id = 0)
        {
            try
            {
                var elements = await _service.GetChemicalElementsBySpecificationsAsync(spec1Id, spec2Id);
                return Ok(elements);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetChemicalElementsBySpecifications] Error: {ex.Message}");
                return StatusCode(500, "Error fetching chemical parameters for specifications.");
            }
        }

        [HttpGet("GetMechanicalLimitsByGrade")]
        public async Task<IActionResult> GetMechanicalLimitsByGrade([FromQuery] long gradeId, [FromQuery] long? gradeId2 = null)
        {
            var gradeIds = new List<long> { gradeId };
            if (gradeId2.HasValue && gradeId2 > 0) gradeIds.Add(gradeId2.Value);

            var lines = await _context.SpecificationLines
                .Where(l => l.SpecificationGradeID.HasValue
                            && gradeIds.Contains(l.SpecificationGradeID.Value)
                            && l.Type != null && l.Type.ToLower() == "mechanical")
                .Select(l => new {
                    SpecificationLineID = l.ID,
                    l.ParameterID,
                    ParameterName = l.Parameter != null ? l.Parameter.Name : null,
                    l.MinValue,
                    l.MaxValue,
                    l.MinTolerance,
                    l.MaxTolerance,
                    UnitName = l.ParameterUnit != null ? l.ParameterUnit.Name : null
                })
                .ToListAsync();
            return Ok(lines);
        }
    }
}
