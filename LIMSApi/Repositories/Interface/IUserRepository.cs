using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<UserMaster> GetUserByEmail(string email);
        Task<List<UserMaster>> GetAllUserByRoleId(long Id);
        Task AddUser(UserMaster user);
        Task UpdateUser(UserMaster user);
        Task UpdateUsers(List<UserMaster> users);
        Task<bool> DeleteUser(string email);
        Task<List<DropdwonSelector>> GetUserDropdown(string? searchTerm, int pageNo, int pageSize);

    }
}
