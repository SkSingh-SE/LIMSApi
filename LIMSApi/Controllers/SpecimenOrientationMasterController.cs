using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecimenOrientationMasterController : ControllerBase
    {
        private readonly ISpecimenOrientationService _specimenService;
        private readonly LIMSContext _context;

        public SpecimenOrientationMasterController(ISpecimenOrientationService specimenRepo, LIMSContext context)
        {
            _specimenService = specimenRepo;
            _context = context;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SpecimenOrientationList(PageFilter filter)
        {
            return Ok(await _specimenService.FetchSpecimenOrientationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecimenOrientationMaster>> GetSpecimenOrientationMaster(long id)
        {
            var entity = await _specimenService.GetSpecimenOrientationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSpecimenOrientationMaster(SpecimenOrientationMaster model)
        {
            await _specimenService.ModifySpecimenOrientation(model);
            return Ok(new
            {
                status = "success",
                message = $"SpecimenOrientation '{model.Name}' updated successfully."
            });
            
        }

        [HttpPost("create")]
        public async Task<ActionResult<SpecimenOrientationMaster>> PostSpecimenOrientationMaster(SpecimenOrientationMaster model)
        {
            await _specimenService.CreateSpecimenOrientation(model);
            return Ok(new
            {
                status = "success",
                message = $"SpecimenOrientation '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecimenOrientationMaster(long id)
        {
            var entity = await _specimenService.GetSpecimenOrientationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SpecimenOrientation not found!");
            }
            await _specimenService.RemoveSpecimenOrientation(id);
            return Ok(new
            {
                status = "success",
                message = $"SpecimenOrientation '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecimenOrientationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _specimenService.GetSpecimenOrientationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

        [HttpGet("by-classification/{metalClassificationId}")]
        public async Task<IActionResult> GetByClassification(long metalClassificationId, string? searchTerm, int pageNo = 1, int pageSize = 20)
        {
            // Check if any mappings exist for this metal classification
            var hasMappings = await _context.SpecimenOrientationMetalClassifications
                .AnyAsync(x => x.MetalClassificationID == metalClassificationId);

            List<DropdwonSelector> data;

            if (hasMappings)
            {
                // Return only orientations mapped to this classification
                var query = _context.SpecimenOrientationMasters
                    .Where(so => so.IsActive &&
                        so.ApplicableClassifications.Any(ac => ac.MetalClassificationID == metalClassificationId));

                if (!string.IsNullOrWhiteSpace(searchTerm))
                    query = query.Where(so => so.Name.Contains(searchTerm));

                data = await query
                    .OrderBy(so => so.Name)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .Select(so => new DropdwonSelector { Id = so.ID, Name = so.Name })
                    .ToListAsync();
            }
            else
            {
                // No mappings — return all orientations
                data = await _specimenService.GetSpecimenOrientationDropdown(searchTerm, pageNo, pageSize);
            }

            return data == null ? NoContent() : Ok(data);
        }

    }
}
