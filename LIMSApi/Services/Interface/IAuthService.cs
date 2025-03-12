using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IAuthService
    {
        Task<string> Authenticate(LoginDTO login);
        Task RegisterUser(UserMaster model);
    }
}
