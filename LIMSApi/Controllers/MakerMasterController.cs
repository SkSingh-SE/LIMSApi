using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakerMasterController : ControllerBase
    {
        private readonly IMakerService _makerService;

        public MakerMasterController(IMakerService makerService)
        {
            _makerService = makerService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> MakerList(PageFilter filter)
        {
            return Ok(await _makerService.FetchMakerList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<MakerMaster>> GetMakerMaster(long id)
        {
            var entity = await _makerService.GetMakerDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutMakerMaster(MakerMaster model)
        {
            await _makerService.ModifyMaker(model);
            return Ok($"Maker '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<MakerMaster>> PostMakerMaster(MakerMaster model)
        {
            await _makerService.CreateMaker(model);
            return Ok($"Maker '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMakerMaster(long id)
        {
            var entity = await _makerService.GetMakerDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Maker not found!");
            }
            return Ok($"Maker '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetMakerDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _makerService.GetMakerDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
