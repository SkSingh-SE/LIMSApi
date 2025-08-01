using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IAuthService
    {
        Task<object> Authenticate(LoginDTO login);
        Task RegisterUser(UserMaster model);
        Task<object> GetRefreshToken();
        string GetHashedPassword(string password);
    }
}
