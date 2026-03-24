using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;
        private readonly IUserService userService;
        private readonly LIMSContext _context;

        public RoleService(IRoleRepository roleRepo, ILogger<RoleService> logger, IUserService userService, LIMSContext context)
        {
            _roleRepository = roleRepo;
            _logger = logger;
            this.userService = userService;
            _context = context;
        }

        public async Task CreateRole(RoleMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Role name should not be empty!");

            bool exists = await _roleRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Role already exists!");

            await _roleRepository.AddRole(model);
            _logger.LogInformation("Role '{RoleName}' created successfully.", model.Name);
        }

        public async Task ModifyRole(RoleMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Role ID should not be empty!");

            bool exists = await _roleRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Role already exists!");

            var existingRole = await _roleRepository.GetRoleById(model.ID);
            if (existingRole == null)
                throw new InvalidOperationException("Role not found!");

            var users = await userService.GetAllUserByRoleId(model.ID);
            if (existingRole.IsAdmin != model.IsAdmin)
            {
                foreach (var user in users)
                {
                    user.IsAdmin = model.IsAdmin;
                }
            }

            existingRole.Name = model.Name;
            existingRole.Description = model.Description;
            existingRole.IsAdmin = model.IsAdmin;
            existingRole.ModifiedOn = DateTime.UtcNow;

            // Clear and update role-menu mappings
            existingRole.MenuItems.Clear();

            foreach (var item in model.MenuItems)
            {
                existingRole.MenuItems.Add(new RoleMenuMapping
                {
                    RoleID = model.ID,
                    MenuID = item.MenuID
                });
            }

            await _roleRepository.UpdateRole(existingRole);
            await userService.UpdateUsers(users);
            _logger.LogInformation("Role '{RoleName}' updated successfully.", model.Name);
        }

        public async Task RemoveRole(long id)
        {
            var existingRole = await _roleRepository.GetRoleById(id);
            if (existingRole == null)
                throw new InvalidOperationException("Role not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<RoleMaster>(_context, id, "Role");

            existingRole.IsActive = false;
            existingRole.ModifiedOn = DateTime.UtcNow;

            await _roleRepository.UpdateRole(existingRole);
            _logger.LogInformation("Role with ID '{RoleId}' deleted successfully.", id);
        }

        public async Task<RoleMaster> GetRoleDetails(long id)
        {
            var classification = await _roleRepository.GetRoleById(id);
            if (classification == null)
                throw new InvalidOperationException("Role not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchRoleList(PageFilter filter)
        {
            return await _roleRepository.GetAllRoles(filter);
        }

        public async Task<List<DropdwonSelector>> GetRoleDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _roleRepository.GetRoleDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
