using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DimensionalFactorMasterController : ControllerBase
    {
        private readonly IDimensionalFactorService _dimensionalFactorService;

        public DimensionalFactorMasterController(IDimensionalFactorService dimensionalFactorRepo)
        {
            _dimensionalFactorService = dimensionalFactorRepo;
        }

        [HttpPost("list")]
        public async Task<IActionResult> DimensionalFactorList(PageFilter filter)
        {
            return Ok(await _dimensionalFactorService.FetchDimensionalFactorList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<DimensionalFactorMaster>> GetDimensionalFactorMaster(long id)
        {
            var entity = await _dimensionalFactorService.GetDimensionalFactorDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutDimensionalFactorMaster(DimensionalFactorMaster model)
        {
            await _dimensionalFactorService.ModifyDimensionalFactor(model);
            return Ok($"DimensionalFactor '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<DimensionalFactorMaster>> PostDimensionalFactorMaster(DimensionalFactorMaster model)
        {
            await _dimensionalFactorService.CreateDimensionalFactor(model);
            return Ok($"DimensionalFactor '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDimensionalFactorMaster(long id)
        {
            var entity = await _dimensionalFactorService.GetDimensionalFactorDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("DimensionalFactor not found!");
            }
            return Ok($"DimensionalFactor '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDimensionalFactorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _dimensionalFactorService.GetDimensionalFactorDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
