using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SampleInwardController : ControllerBase
    {
        private readonly ISampleInwardService _SampleInwardService;

        public SampleInwardController(ISampleInwardService SampleInwardServce)
        {
            _SampleInwardService = SampleInwardServce;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SampleInwardList(PageFilter filter)
        {
            return Ok(await _SampleInwardService.FetchSampleInwardList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetSampleInward(long id)
        {
            var entity = await _SampleInwardService.GetSampleInwardDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSampleInward([FromForm] SampleInwardDto model)
        {
            await _SampleInwardService.ModifySampleInward(model);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<SampleInward>> PostSampleInward([FromForm] SampleInwardDto model)
        {
            await _SampleInwardService.CreateSampleInward(model);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSampleInward(long id)
        {
            var entity = await _SampleInwardService.GetSampleInwardDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SampleInward not found!");
            }
            await _SampleInwardService.RemoveSampleInward(id);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward deleted successfully."
            });
        }

        [HttpGet("case-number")]
        public async Task<IActionResult> GetCaseNoAndSampleNo()
        {
            var caseNumber = await _SampleInwardService.GetCaseNoAndSampleNo();

            return caseNumber == null ? NoContent() : Ok(caseNumber);
        }
    }
}
