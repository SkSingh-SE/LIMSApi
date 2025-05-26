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
    public class DisciplineMasterController : ControllerBase
    {
        private readonly IDisciplineService _DisciplineService;

        public DisciplineMasterController(IDisciplineService DisciplineService)
        {
            _DisciplineService = DisciplineService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> DisciplineList(PageFilter filter)
        {
            return Ok(await _DisciplineService.FetchDisciplineList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<DisciplineMaster>> GetDisciplineMaster(long id)
        {
            var entity = await _DisciplineService.GetDisciplineDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutDisciplineMaster(DisciplineMaster model)
        {
            await _DisciplineService.ModifyDiscipline(model);
            return Ok(new
            {
                status = "success",
                message = $"DisciplineMaster '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<DisciplineMaster>> PostDisciplineMaster(DisciplineMaster model)
        {
            await _DisciplineService.CreateDiscipline(model);
            return Ok(new
            {
                status = "success",
                message = $"DisciplineMaster '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDisciplineMaster(long id)
        {
            var entity = await _DisciplineService.GetDisciplineDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("DisciplineMaster not found!");
            }
            await _DisciplineService.RemoveDiscipline(id);
            return Ok(new
            {
                status = "success",
                message = $"DisciplineMaster '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDisciplineDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _DisciplineService.GetDisciplineDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
