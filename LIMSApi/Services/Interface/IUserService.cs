using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IUserService
    {
        Task UpdateUser(UserMaster user);
        Task<bool> DeleteUser(string email);
        Task<UserMaster> GetUserByEmail(string email);

    }
}
