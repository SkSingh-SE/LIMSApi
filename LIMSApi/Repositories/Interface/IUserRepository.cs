using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<UserMaster> GetUserByEmail(string email);
        Task AddUser(UserMaster user);
        Task UpdateUser(UserMaster user);
        Task<bool> DeleteUser(string email);

    }
}
