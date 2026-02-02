using System.Security.Claims;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace LIMSApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IAuthService authService;
        private readonly EmailService _emailService;
        private readonly TemplateService _templateService;
        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IAuthService authService, EmailService emailService, TemplateService templateService)
        {
            _logger = logger;
            _userRepository = userRepository;
            this.authService = authService;
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task<bool> DeleteUser(string email)
        {
            bool result = await _userRepository.DeleteUser(email);
            if (result)
            {
            _logger.LogInformation("User {email} deleted successfully", email);
            }
            else
            {
                _logger.LogWarning("User not associated with : {email}", email);
            }
            return result;
        }

        public async Task UpdateUser(UserMaster user)
        {
            await _userRepository.UpdateUser(user);
            _logger.LogInformation("User {Username} updated successfully", user.UserName);
        }
        public async Task UpdateUsers(List<UserMaster> users)
        {
            await _userRepository.UpdateUsers(users);
        }

        public async Task<UserMaster> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                _logger.LogWarning("User not found : {Email}", email);
                return null;
            }
            return user;
        }

        public async Task<List<UserMaster>> GetAllUserByRoleId(long Id)
        {
            return await _userRepository.GetAllUserByRoleId(Id);
        }

        public async Task<List<DropdwonSelector>> GetUserDropdown(string? searchTerm, int pageNo, int pageSize)
        {
           var users = await _userRepository.GetUserDropdown(searchTerm, pageNo, pageSize);
            if (users == null || users.Count == 0)
            {
                _logger.LogWarning("No users found for search term: {SearchTerm}", searchTerm);
                return new List<DropdwonSelector>();
            }
            _logger.LogInformation("{Count} users found for search term: {SearchTerm}", users.Count, searchTerm);
            return users;
        }

        public async Task<UserAccountDto> GetByEmployee(long employeeId)
        {
            var user = await _userRepository.GetByEmployee(employeeId);
            return new UserAccountDto
            {
                UserName = user.UserName,
                Email = user.EmailId ?? string.Empty,
                IsLoginEnabled = user.IsLoginEnabled,
                AccountStatus = string.IsNullOrEmpty(user.AccountStatus) ? user.IsActive ? "Active" : "In-Active" : user.AccountStatus,
                LastLoginDate = user.LastLoginDate,
                FailedLoginAttempts = user.FailedLoginAttempts,
                CurrentRole = user.RoleName ?? user.Role?.Name ?? string.Empty,
                SessionTimeout = user.SessionTimeout,
                TwoFactorEnabled = user.TwoFactorEnabled,
                TwoFactorMethod = user.TwoFactorMethod,
                AutoLockAfterAttempts = user.AutoLockAfterAttempts,
                UnlockMethod = string.IsNullOrEmpty(user.UnlockMethod) ? "Admin" : user.UnlockMethod,
                AllowRemoteLogin = user.RemoteLogin,
                IpRestriction = user.IpRestriction,
                WorkingHours = user.WorkingHours
            };
        }
        public async Task UpdateByEmployee(long employeeId, UserAccountDto dto)
        {
            await _userRepository.UpdateByEmployee(employeeId, dto);
        }

        public async Task ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userRepository.GetByEmployee(dto.EmployeeId);
            if (user is null)
                throw new InvalidOperationException("Employee Not mapped with User");

            user.Password = authService.GetHashedPassword(dto.NewPassword);
            user.FailedLoginAttempts = 0;
            user.AccountStatus = "Active";

            await _userRepository.UpdateUser(user);

        }

        public async Task SendTwoFactorOtp(Send2FADto dto)
        {
            var user = await _userRepository.GetByEmployee(dto.EmployeeId);

            if (user is null)
                throw new KeyNotFoundException("User not found");

            if (dto.Method is not ("email" or "sms"))
                throw new ArgumentException("Invalid 2FA method");

            var now = DateTime.UtcNow;

            if (user.TwoFactorLastSentAt.HasValue &&
                (now - user.TwoFactorLastSentAt.Value).TotalSeconds < 60)
                throw new InvalidOperationException("Please wait before resending OTP");

            var otp = Random.Shared.Next(100000, 999999).ToString();

            user.TwoFactorOtp = otp;
            user.TwoFactorOtpExpiry = now.AddMinutes(5);
            user.TwoFactorMethod = dto.Method;
            user.TwoFactorLastSentAt = now;
            user.TwoFactorSendCount++;

            await _userRepository.UpdateUser(user);

            if (dto.Method == "email" || true)
            {
                var subject = "Your One-Time Verification Code";

                var template = _templateService.GetMessageBody(MessageTemplateKey.SEND_OTP);

                var body = template
                    .Replace("{UserName}", user.UserName)
                    .Replace("{OTP}", otp)
                    .Replace("{ApplicationName}", "Divine Metallurgical Services Pvt. Ltd.");

                await _emailService.SendEmailAsync(
                    user.EmailId!,
                    subject,
                    body
                );
            }

            //if (dto.Method == "sms")
            //    await _smsService.SendAsync(
            //        user.MobileNo!,
            //        $"Your OTP is {otp}"
            //    );

            _logger.LogInformation(
                "2FA OTP sent via {Method} for UserId {UserId}",
                dto.Method,
                user.ID
            );
        }

        public async Task VerifyTwoFactorOtp(Verify2FADto dto)
        {
            var user = await _userRepository.GetByEmployee(dto.EmployeeId);

            if (user is null)
                throw new KeyNotFoundException("User not found");

            if (user.TwoFactorOtp != dto.Otp ||
                user.TwoFactorOtpExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired OTP");

            user.TwoFactorEnabled = true;
            user.TwoFactorOtp = null;
            user.TwoFactorOtpExpiry = null;

            await _userRepository.UpdateUser(user);

            _logger.LogInformation(
                "2FA enabled for UserId {UserId}",
                user.ID
            );
        }
    }
}
