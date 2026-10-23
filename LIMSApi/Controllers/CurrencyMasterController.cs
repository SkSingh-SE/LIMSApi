using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyMasterController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyMasterController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CurrencyList(PageFilter filter)
        {
            return Ok(await _currencyService.FetchCurrencies(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<CurrencyMaster>> GetCurrencyMaster(long id)
        {
            var entity = await _currencyService.GetCurrencyDetails(id);

            return entity== null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutCurrencyMaster(CurrencyMaster model)
        {
            await _currencyService.ModifyCurrency(model);
            return Ok($"Currency '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<CurrencyMaster>> PostCurrencyMaster(CurrencyMaster model)
        {
            await _currencyService.CreateCurrency(model);
            return Ok($"Currency '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCurrencyMaster(long id)
        {
            var entity = await _currencyService.GetCurrencyDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Currency not found!");
            }
            return Ok($"Currency '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCurrencyDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _currencyService.GetCurrencyDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultCurrency()
        {
            var currency = await _currencyService.GetDefaultCurrency();
            return currency == null ? NoContent() : Ok(new { id = currency.ID, name = currency.Code != null ? $"{currency.Name}-({currency.Code})" : currency.Name });
        }

        [HttpPut("set-default/{id}")]
        public async Task<IActionResult> SetDefaultCurrency(long id)
        {
            await _currencyService.SetDefaultCurrency(id);
            return Ok(new { message = "Default currency updated successfully." });
        }

    }
}
