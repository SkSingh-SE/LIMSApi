using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubGroupMasterController : ControllerBase
    {
        private readonly ISubGroupService _subGroupService;

        public SubGroupMasterController(ISubGroupService subGroupService)
        {
            _subGroupService = subGroupService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SubGroupList(PageFilter filter)
        {
            return Ok(await _subGroupService.FetchSubGroupList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<SubGroupMaster>> GetSubGroupMaster(long id)
        {
            var entity = await _subGroupService.GetSubGroupDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSubGroupMaster(SubGroupMaster model)
        {
            await _subGroupService.ModifySubGroup(model);
            return Ok(new
            {
                status = "success",
                message = $"SubGroup Master '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<SubGroupMaster>> PostSubGroupMaster(SubGroupMaster model)
        {
            await _subGroupService.CreateSubGroup(model);
            return Ok(new
            {
                status = "success",
                message = $"SubGroup Master '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSubGroupMaster(long id)
        {
            var entity = await _subGroupService.GetSubGroupDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SubGroupMaster not found!");
            }
            await _subGroupService.RemoveSubGroup(id);
            return Ok(new
            {
                status = "success",
                message = $"SubGroup Master '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSubGroupDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _subGroupService.GetSubGroupDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }
        [HttpGet("dropdownByGroupId")]
        public async Task<IActionResult> GetDropdownByGroupId(long groupId)
        {
            var data = await _subGroupService.GetDropdownByGroupId(groupId);
            return data == null ? NoContent() : Ok(data);
        }

    }
}
