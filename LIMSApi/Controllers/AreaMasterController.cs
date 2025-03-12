using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaMasterController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaMasterController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> AreaList(PageFilter filter)
        {
            return Ok(await _areaService.FetchAreas(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<AreaMaster>> GetAreaMaster(long id)
        {
            var area = await _areaService.GetAreaDetails(id);

            return area;
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutAreaMaster(AreaMaster model)
        {
            await _areaService.ModifyArea(model);
            return Ok($"Area '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<AreaMaster>> PostAreaMaster(AreaMaster model)
        {
            await _areaService.CreateArea(model);
            return Ok($"Area '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAreaMaster(long id)
        {
            var area = await _areaService.GetAreaDetails(id);
            if (area == null)
            {
                throw new InvalidOperationException("Area not found!");
            }
            return Ok($"Area '{area.Name}' created successfully");
        }

        [HttpGet("getAreaWithPincode")]
        public async Task <IActionResult> GetAreaWithPincode(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _areaService.GetAreaWithPincode(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}

