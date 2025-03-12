using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationMasterController : ControllerBase
    {
        private readonly IClassificationService _classificationService;

        public ClassificationMasterController(IClassificationService classificationService)
        {
            _classificationService = classificationService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ClassificationList(PageFilter filter)
        {
            return Ok(await _classificationService.FetchClassificationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<ClassificationMaster>> GetClassificationMaster(long id)
        {
            var entity = await _classificationService.GetClassificationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutClassificationMaster(ClassificationMaster model)
        {
            await _classificationService.ModifyClassification(model);
            return Ok($"Classification '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<ClassificationMaster>> PostClassificationMaster(ClassificationMaster model)
        {
            await _classificationService.CreateClassification(model);
            return Ok($"Classification '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteClassificationMaster(long id)
        {
            var entity = await _classificationService.GetClassificationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Classification not found!");
            }
            return Ok($"Classification '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetClassificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _classificationService.GetClassificationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
