using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IUserService
    {
        Task UpdateUser(UserMaster user);
        Task UpdateUsers(List<UserMaster> users);
        Task<bool> DeleteUser(string email);
        Task<UserMaster> GetUserByEmail(string email);
        Task<List<UserMaster>> GetAllUserByRoleId(long Id);

        Task<List<DropdwonSelector>> GetUserDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<UserAccountDto> GetByEmployee(long employeeId);
        Task UpdateByEmployee(long employeeId, UserAccountDto dto);
        Task ResetPassword(ResetPasswordDto dto);
        Task SendTwoFactorOtp(Send2FADto dto);
        Task VerifyTwoFactorOtp(Verify2FADto dto);

    }
}
