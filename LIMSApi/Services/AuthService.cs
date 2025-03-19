using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace LIMSApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly string _jwtSecret;
        private readonly PasswordHasher<UserMaster> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger, string jwtSecret, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtSecret = jwtSecret;
            _passwordHasher = new PasswordHasher<UserMaster>();
            _configuration = configuration;
        }
        public static DateTimeOffset ConvertToTimeZone(DateTime utcDateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }
        public async Task<string> Authenticate(LoginDTO login)
        {
            var date1 = DateTime.UtcNow;
            var date2 = DateTime.Now;
            var date3 = DateTimeOffset.Now;
            var date4 = DateTimeOffset.UtcNow;

            Console.WriteLine(date1);
            Console.WriteLine( date2);
            Console.WriteLine(date3);
            Console.WriteLine(date4);

            DateTimeOffset localTime = ConvertToTimeZone(date1, "India Standard Time");
            Console.WriteLine(localTime);

            var user = await _userRepository.GetUserByEmail(login.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"Login failed for user: {login.Email}");
            }
            
            // Validate password hash
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidCredentialException($"Invalid password for user: {user.UserName}");
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation("User {Username} logged in successfully", user.UserName);

            return token;
        }

        public async Task RegisterUser(UserMaster model)
        {
            var existingUser = await _userRepository.GetUserByEmail(model.EmailId);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            var user = new UserMaster
            {
                UserName = model.UserName,
                EmailId = model.EmailId,
                Password = _passwordHasher.HashPassword(null, model.Password)
            };

            await _userRepository.AddUser(user);
            _logger.LogInformation("User {Username} registered successfully", model.UserName);
        }

        private string GenerateJwtToken(UserMaster user)
        {
            var expireHours = Convert.ToInt32(_configuration["Jwt:ExpirationHours"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleID != null && user.RoleID != 0 ? user.RoleID.ToString() : ""),
                    new Claim(ClaimTypes.Email, user.EmailId ?? string.Empty),
                    new Claim("EmployeeID", user.EmployeeID != null ? user.EmployeeID.ToString() : "0"),
                    new Claim("CompanyCode", user.CompanyCode ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddHours(expireHours),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

           
        }
    }
}
