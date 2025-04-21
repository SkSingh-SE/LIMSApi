using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
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
        private readonly EmailService emailService;
        private LoggedInUserDTO loggedInUserDTO;
        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger, string jwtSecret, IConfiguration configuration, EmailService emailService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtSecret = jwtSecret;
            _passwordHasher = new PasswordHasher<UserMaster>();
            _configuration = configuration;
            this.emailService = emailService;
            loggedInUserDTO = LoggedInUserProvider.CurrentUser;
        }
        public static DateTimeOffset ConvertToTimeZone(DateTime utcDateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }
        public async Task<object> Authenticate(LoginDTO login)
        {
            var date1 = DateTime.UtcNow;
            var date2 = DateTime.Now;
            var date3 = DateTimeOffset.Now;
            var date4 = DateTimeOffset.UtcNow;

            Console.WriteLine(date1);
            Console.WriteLine(date2);
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

            var expireHours = Convert.ToInt32(_configuration["Jwt:ExpirationHours"]);
            var responseObject = new
            {
                token = token,
                name = user.UserName,
                email = user.EmailId,
                role = user.RoleName,
                expiresInSecond = expireHours * 60 * 60,
                employeeId = user.EmployeeID
            };
            return responseObject;
        }

        public async Task<object> GetRefreshToken()
        {
            //var oldToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            //var oldToken = loggedInUserDTO?.Email;
            //if (IsTokenExpired(oldToken))
            //{
            //    throw new UnauthorizedAccessException("Token has expired. Please login again.");
            //}

            if (loggedInUserDTO != null)
            {
                var user = await _userRepository.GetUserByEmail(loggedInUserDTO.Email);
                if (user == null)
                {
                    throw new UnauthorizedAccessException($"User not found: {loggedInUserDTO.Email}");
                }

                var token = GenerateJwtToken(user);

                var expireHours = Convert.ToInt32(_configuration["Jwt:ExpirationHours"]);
                var responseObject = new
                {
                    token = token,
                    name = user.UserName,
                    email = user.EmailId,
                    role = user.RoleName,
                    expiresInSecond = expireHours  * 60 * 60,
                    employeeId = user.EmployeeID
                };
                return responseObject;
            }
            return null;
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
                Password = _passwordHasher.HashPassword(null, model.Password),
                EmployeeID = model.EmployeeID,
                RoleID = model.RoleID,
                RoleName = model.RoleName,
                CompanyCode = model.CompanyCode
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
        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                throw new ArgumentException("Invalid JWT token format.");

            var jwtToken = handler.ReadJwtToken(token);
            var expiry = jwtToken.ValidTo;

            return expiry < DateTime.UtcNow;
        }

    }
}
