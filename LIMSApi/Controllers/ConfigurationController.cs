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
using Microsoft.AspNetCore.Authorization;
using LIMSApi.Dtos;
using LIMSApi.Services;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigurationsController : ControllerBase
    {
        private readonly IConfigurationService _ConfigurationService;
        private readonly ILogger<ConfigurationsController> _logger;

        public ConfigurationsController(IConfigurationService ConfigurationService, ILogger<ConfigurationsController> logger)
        {
            _ConfigurationService = ConfigurationService;
            _logger = logger;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ConfigurationList(PageFilter filter)
        {
            return Ok(await _ConfigurationService.FetchConfigurationList(filter));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateConfiguration(Configuration Configuration)
        {
            await _ConfigurationService.CreateConfiguration(Configuration);
            return Ok(new
            {
                status = "success",
                message = $"Configuration '{Configuration.KeyName}' created successfully."
            });
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetConfigurationById(long Id)
        {
            var Configuration = await _ConfigurationService.GetConfigurationById(Id);

            if (Configuration == null)
            {
                return NotFound($"Configuration not found : {Id}");
            }

            return Ok(Configuration);
        }
        [HttpGet("get/{key}")]
        public async Task<IActionResult> GetConfigurationByKey(string key)
        {
            var Configuration = await _ConfigurationService.GetConfigurationByKey(key);

            if (Configuration == null)
            {
                return NotFound($"Configuration not found : {key}");
            }

            return Ok(Configuration);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateConfiguration(Configuration Configuration)
        {
            await _ConfigurationService.UpdateConfiguration(Configuration);
            return Ok(new
            {
                status = "success",
                message = $"Configuration '{Configuration.KeyName}' updated successfully."
            });
        }

        [HttpGet("get-value/{key}")]
        public async Task<IActionResult> GetConfigurationValueByKey(string key)
        {
            var configValue = await _ConfigurationService.GetConfigurationValueByKey(key);

            if (configValue == null)
            {
                return NotFound($"No Value added for that key : {key}");
            }

            return Ok(configValue);
        }
    }
}
