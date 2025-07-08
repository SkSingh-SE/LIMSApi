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

        [HttpGet("get/{key}")]
        public async Task<IActionResult> GetConfigurationByEmail(string key)
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

    }
}
