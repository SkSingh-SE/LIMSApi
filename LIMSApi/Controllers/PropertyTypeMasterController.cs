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
    public class PropertyTypeMasterController : ControllerBase
    {
        private readonly IPropertyTypeService _propertyTypeService;

        public PropertyTypeMasterController(IPropertyTypeService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> PropertyTypeList(PageFilter filter)
        {
            return Ok(await _propertyTypeService.FetchPropertyTypeList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<PropertyTypeMaster>> GetPropertyTypeMaster(long id)
        {
            var entity = await _propertyTypeService.GetPropertyTypeDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutPropertyTypeMaster(PropertyTypeMaster model)
        {
            await _propertyTypeService.ModifyPropertyType(model);
            return Ok(new
            {
                status = "success",
                message = $"PropertyType '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<PropertyTypeMaster>> PostPropertyTypeMaster(PropertyTypeMaster model)
        {
            await _propertyTypeService.CreatePropertyType(model);
            return Ok(new
            {
                status = "success",
                message = $"PropertyType '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePropertyTypeMaster(long id)
        {
            var entity = await _propertyTypeService.GetPropertyTypeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("PropertyType not found!");
            }
            await _propertyTypeService.RemovePropertyType(id);
            return Ok(new
            {
                status = "success",
                message = $"PropertyType '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetPropertyTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _propertyTypeService.GetPropertyTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
