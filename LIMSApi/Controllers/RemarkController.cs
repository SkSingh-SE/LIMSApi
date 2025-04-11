using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemarkController : ControllerBase
    {
        private readonly IRemarkService _remarkService;

        public RemarkController(IRemarkService remarkService)
        {
            _remarkService = remarkService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> RemarkList(PageFilter filter)
        {
            return Ok(await _remarkService.FetchRemarkList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<RemarkMaster>> GetRemarkMaster(long id)
        {
            var entity = await _remarkService.GetRemarkDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutRemarkMaster(RemarkMaster model)
        {
            await _remarkService.ModifyRemark(model);
            return Ok($"RemarkMaster '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<RemarkMaster>> PostRemarkMaster(RemarkMaster model)
        {
            await _remarkService.CreateRemark(model);
            return Ok($"RemarkMaster '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRemarkMaster(long id)
        {
            var entity = await _remarkService.GetRemarkDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("RemarkMaster not found!");
            }
            return Ok($"RemarkMaster '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetRemarkDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _remarkService.GetRemarkDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
