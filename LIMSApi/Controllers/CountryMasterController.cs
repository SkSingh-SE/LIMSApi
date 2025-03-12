using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using LIMSApi.Dtos;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryMasterController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryMasterController(ICountryService countryS)
        {
            _countryService = countryS;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CountryList(PageFilter filter)
        {
            return Ok(_countryService.FetchCountries(filter));
        }

        
        [HttpGet("details/{id}")]
        public async Task<ActionResult<CountryMaster>> GetCountryMaster(long id)
        {
            var countryMaster = await _countryService.GetCountryDetails(id);
            
            return countryMaster;
        }

       
        [HttpPut("update")]
        public async Task<IActionResult> PutCountryMaster(CountryMaster countryMaster)
        {
            await _countryService.ModifyCountry(countryMaster);
            return Ok($"Country '{countryMaster.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<CountryMaster>> PostCountryMaster(CountryMaster countryMaster)
        {
            await _countryService.CreateCountry(countryMaster);
            return Ok($"Country '{countryMaster.Name}' created successfully");
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCountryMaster(long id)
        {
            var countryMaster = await _countryService.GetCountryDetails(id);
            if(countryMaster == null)
            {
                throw new InvalidOperationException("Country not found!");
            }
            return Ok($"Country '{countryMaster.Name}' created successfully");
        }

    }
}
