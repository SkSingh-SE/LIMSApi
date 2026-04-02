using System.Linq.Dynamic.Core;
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
        public async Task<Configuration> GetConfigurationById(long Id)
        {
            var Configuration = await context.Configurations.FirstOrDefaultAsync(x => x.ID == Id && x.CompanyCode == loggedInUser.CompanyCode);
            return Configuration;
        }
        public async Task UpdateConfiguration(Configuration Configuration)
        {
            context.Update(Configuration);
            await context.SaveChangesAsync();
        }
        public async Task<PagedResponse<object>> GetAllConfigurations(PageFilter filter)
        {
            var _query = from c in context.Configurations where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.KeyName != null && x.KeyName.ToLower().Contains(search))
                    || (x.GroupName != null && x.GroupName.ToLower().Contains(search))
                    || (x.Value != null && x.Value.ToLower().Contains(search))
                    || (x.ValueType != null && x.ValueType.ToLower().Contains(search))
                    || (x.Description != null && x.Description.ToLower().Contains(search))
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<bool> IsExistKeyName(string key)
        {
            return await context.Configurations.AnyAsync(x => x.KeyName == key && x.CompanyCode == loggedInUser.CompanyCode && x.IsActive);
        }

        public async Task<bool> IsExistKeyAndId(string key, long Id)
        {
            return await context.Configurations.AnyAsync(x => x.KeyName == key && x.ID != Id && x.CompanyCode == loggedInUser.CompanyCode && x.IsActive);
        }
        public async Task<List<string>> GetConfigurationValueByKey(string Key)
        {
            var config = await context.Configurations.FirstOrDefaultAsync(x => x.KeyName == Key);
            if(config == null)
            {
                throw new InvalidOperationException($"Data not found related to {Key} key");
            }
            if(config?.GroupName == "dropdown")
            {
                var ddata = config.Value.Split("|").ToList();
                return ddata;
            }
            else
            {
                var data = new List<string>();
                data.Add(config.Value);
                return data;
            }
        }
    }
}
