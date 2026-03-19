using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplePreparationMasterController : ControllerBase
    {
        private readonly ISamplePreparationMasterService _service;

        public SamplePreparationMasterController(ISamplePreparationMasterService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SamplePreparationMasterList(PageFilter filter)
        {
            return Ok(await _service.FetchSamplePreparationMasterList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<SamplePreparationMaster>> GetSamplePreparationMaster(long id)
        {
            var entity = await _service.GetSamplePreparationMasterDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutSamplePreparationMaster(SamplePreparationMaster model)
        {
            await _service.ModifySamplePreparationMaster(model);
            return Ok(new
            {
                status = "success",
                message = $"SamplePreparationMaster '{model.SpecimenType}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<SamplePreparationMaster>> PostSamplePreparationMaster(SamplePreparationMaster model)
        {
            await _service.CreateSamplePreparationMaster(model);
            return Ok(new
            {
                status = "success",
                message = $"SamplePreparationMaster '{model.SpecimenType}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSamplePreparationMaster(long id)
        {
            var entity = await _service.GetSamplePreparationMasterDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SamplePreparationMaster not found!");
            }
            await _service.RemoveSamplePreparationMaster(id);
            return Ok(new
            {
                status = "success",
                message = $"SamplePreparationMaster '{entity.SpecimenType}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSamplePreparationMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _service.GetSamplePreparationMasterDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("by-test/{laboratoryTestId}")]
        public async Task<IActionResult> GetByLaboratoryTest(long laboratoryTestId)
        {
            var data = await _service.GetByLaboratoryTestId(laboratoryTestId);
            return data == null || data.Count == 0 ? NoContent() : Ok(data);
        }
    }
}
