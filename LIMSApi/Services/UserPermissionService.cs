

using LIMSApi.Dtos;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionRepository _UserPermissionRepository;
        private readonly ILogger<UserPermissionService> _logger;

        public UserPermissionService(IUserPermissionRepository UserPermissionRepo, ILogger<UserPermissionService> logger)
        {
            _UserPermissionRepository = UserPermissionRepo;
            _logger = logger;
        }

        public async Task SaveUserPermissions(long userId, List<UserPermissionUpdateDto> updatedPermissions)
        {
            await _UserPermissionRepository.SaveUserPermissions(userId, updatedPermissions);
            _logger.LogInformation("UserPermission '{UserPermissionName}' created successfully.", userId);
        }

        public async Task<List<MenuPermissionGroupDto>> GetUserPermissions(long userId)
        {
            return await _UserPermissionRepository.GetUserPermissions(userId);
        }
    }
}
