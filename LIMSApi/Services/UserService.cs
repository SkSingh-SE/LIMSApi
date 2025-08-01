using System.Security.Claims;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace LIMSApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _logger = logger;
            _userRepository = userRepository;        }

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
    }
}
