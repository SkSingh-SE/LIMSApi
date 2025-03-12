using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityMasterController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityMasterController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CityList(PageFilter filter)
        {
            return Ok(_cityService.FetchCities(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<CityMaster>> GetCityMaster(long id)
        {
            var city = await _cityService.GetCityDetails(id);

            return city;
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutCityMaster(CityMaster model)
        {
            await _cityService.ModifyCity(model);
            return Ok($"City '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<CityMaster>> PostCityMaster(CityMaster model)
        {
            await _cityService.CreateCity(model);
            return Ok($"City '{model.Name}' created successfully");
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCityMaster(long id)
        {
            var city = await _cityService.GetCityDetails(id);
            return city == null ? throw new InvalidOperationException("City not found!") : (IActionResult)Ok($"City '{city.Name}' created successfully");
        }

    }
}
