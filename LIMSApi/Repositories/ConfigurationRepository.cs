using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly LIMSContext context;
        private LoggedInUserDTO loggedInUser;
        public ConfigurationRepository(LIMSContext _context)
        {
            this.context = _context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddConfiguration(Configuration Configuration)
        {
            await context.AddAsync(Configuration);
            await context.SaveChangesAsync();
        }

        public async Task<Configuration> GetConfigurationByKey(string key)
        {
            var Configuration = await context.Configurations.FirstOrDefaultAsync(x => x.KeyName == key && x.CompanyCode == loggedInUser.CompanyCode);
            return Configuration;
        }
        public async Task UpdateConfiguration(Configuration Configuration)
        {
            context.Update(Configuration);
            await context.SaveChangesAsync();
        }

       
    }
}
