using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateMasterController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateMasterController(IStateService countryS)
        {
            _stateService = countryS;
        }

        [HttpPost("list")]
        public async Task<IActionResult> StateList(PageFilter filter)
        {
            return Ok(_stateService.FetchStates(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<StateMaster>> GetStateMaster(long id)
        {
            var state = await _stateService.GetStateDetails(id);

            return state;
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutStateMaster(StateMaster model)
        {
            await _stateService.ModifyState(model);
            return Ok($"State '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<StateMaster>> PostStateMaster(StateMaster model)
        {
            await _stateService.CreateState(model);
            return Ok($"State '{model.Name}' created successfully");
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStateMaster(long id)
        {
            var stateMaster = await _stateService.GetStateDetails(id);
            if (stateMaster == null)
            {
                throw new InvalidOperationException("State not found!");
            }
            return Ok($"State '{stateMaster.Name}' created successfully");
        }

    }
}
