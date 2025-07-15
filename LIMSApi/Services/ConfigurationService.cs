using System.Security.Claims;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace LIMSApi.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _ConfigurationRepository;
        private readonly ILogger<ConfigurationService> _logger;
        public ConfigurationService(IConfigurationRepository ConfigurationRepository, ILogger<ConfigurationService> logger)
        {
            _logger = logger;
            _ConfigurationRepository = ConfigurationRepository;
        }

        public async Task CreateConfiguration(Configuration Configuration)
        {
            var existing = await _ConfigurationRepository.IsExistKeyName(Configuration.KeyName);
            if (existing)
            {
                throw new InvalidOperationException("Key Already Exists");
            }
            await _ConfigurationRepository.AddConfiguration(Configuration);
            _logger.LogInformation("Configuration {KeyName} created successfully", Configuration.KeyName);
        }

        public async Task UpdateConfiguration(Configuration Configuration)
        {
            var existingKey = await _ConfigurationRepository.IsExistKeyAndId(Configuration.KeyName,Configuration.ID);
            if (existingKey)
            {
                throw new InvalidOperationException("Key Already Exists");
            }
            var existing = await _ConfigurationRepository.GetConfigurationByKey(Configuration.KeyName);
            if (existing == null)
            {
                throw new InvalidOperationException("Key Not Found");
            }
            existing.KeyName = Configuration.KeyName;
            existing.GroupName = Configuration.GroupName;
            existing.Value = Configuration.Value;
            existing.ValueType = Configuration.ValueType;
            existing.Description = Configuration.Description;
            existing.ModifiedOn = DateTime.UtcNow;

            await _ConfigurationRepository.UpdateConfiguration(Configuration);
            _logger.LogInformation("Configuration {KeyName} updated successfully", Configuration.KeyName);
        }

        public async Task<Configuration> GetConfigurationByKey(string key)
        {
            var Configuration = await _ConfigurationRepository.GetConfigurationByKey(key);

            if (Configuration == null)
            {
                _logger.LogWarning("Configuration not found : {KeyName}", key);
                throw new KeyNotFoundException($"Configuration with key '{key}' not found.");
            }
            return Configuration;
        }

        public async Task<PagedResponse<object>> FetchConfigurationList(PageFilter filter)
        {
            var configurations = await _ConfigurationRepository.GetAllConfigurations(filter);
            return configurations;
        }
    }
}
