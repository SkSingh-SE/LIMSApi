using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankService _remarkService;

        public BankController(IBankService remarkService)
        {
            _remarkService = remarkService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> BankList(PageFilter filter)
        {
            return Ok(await _remarkService.FetchBankList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<BankMaster>> GetBankMaster(long id)
        {
            var entity = await _remarkService.GetBankDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutBankMaster(BankMaster model)
        {
            await _remarkService.ModifyBank(model);
            return Ok($"BankMaster '{model.BankName}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<BankMaster>> PostBankMaster(BankMaster model)
        {
            await _remarkService.CreateBank(model);
            return Ok($"BankMaster '{model.BankName}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBankMaster(long id)
        {
            var entity = await _remarkService.GetBankDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("BankMaster not found!");
            }
            return Ok($"BankMaster '{entity.BankName}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetBankDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _remarkService.GetBankDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
