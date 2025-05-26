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
    public class GroupMasterController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupMasterController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GroupList(PageFilter filter)
        {
            return Ok(await _groupService.FetchGroupList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<GroupMaster>> GetGroupMaster(long id)
        {
            var entity = await _groupService.GetGroupDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutGroupMaster(GroupMaster model)
        {
            await _groupService.ModifyGroup(model);
            return Ok(new
            {
                status = "success",
                message = $"GroupMaster '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<GroupMaster>> PostGroupMaster(GroupMaster model)
        {
            await _groupService.CreateGroup(model);
            return Ok(new
            {
                status = "success",
                message = $"GroupMaster '{model.Name}' ctreated successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGroupMaster(long id)
        {
            var entity = await _groupService.GetGroupDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("GroupMaster not found!");
            }
            await _groupService.RemoveGroup(id);
            return Ok(new
            {
                status = "success",
                message = $"GroupMaster '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetGroupDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _groupService.GetGroupDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
