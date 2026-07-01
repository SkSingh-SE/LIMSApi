using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        private readonly EmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<AuthService> logger, IUserRepository userRepository, EmailService emailService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _passwordHasher = new PasswordHasher<UserMaster>();
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _jwtSecret = _configuration["Jwt:Secret"] ?? string.Empty;

        }
        public static DateTimeOffset ConvertToTimeZone(DateTime utcDateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }

        public async Task<object> Authenticate(LoginDTO login)
        {
            var nowUtc = DateTime.UtcNow;
            var localTime = ConvertToTimeZone(nowUtc, "India Standard Time");

            var user = await _userRepository.GetUserByEmail(login.Email);

            if (user is null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (user.AccountStatus is "Disabled" or "Locked")
                throw new UnauthorizedAccessException("Account is not accessible");

            if (!user.IsLoginEnabled)
                throw new UnauthorizedAccessException("Login access denied");

            // 🔐 Access policy checks

            var httpContext = _httpContextAccessor.HttpContext!;
            var isRemote = IsRemoteRequest(httpContext);

            if (isRemote && !user.RemoteLogin)
                throw new UnauthorizedAccessException("Access denied");

            if (isRemote)
            {
                if (!IsIpAllowed(user.IpRestriction, httpContext))
                    throw new UnauthorizedAccessException("Access denied");

                if (!IsWithinWorkingHours(user.WorkingHours, localTime))
                    throw new UnauthorizedAccessException("Access denied");
            }

            if (user.ForcePasswordChange && user.SessionTimeout > 0)
            {
                throw new UnauthorizedAccessException("Session Time out, need to change password");
            }
            // Validate password hash
            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.Password!, login.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;

                if (user.AutoLockAfterAttempts > 0 && user.FailedLoginAttempts >= user.AutoLockAfterAttempts)
                {
                    user.AccountStatus = "Locked";
                }

                await _userRepository.UpdateUser(user);
                throw new UnauthorizedAccessException("Invalid credentials");
            }


            user.FailedLoginAttempts = 0;
            user.LastLoginDate = nowUtc;

            // Resolve role via Designation -> Role chain; fallback to direct User.RoleName
            var resolvedRoleName = user.RoleName;
            var resolvedIsAdmin = user.IsAdmin;
            if (user.Employee?.Designation?.Role != null)
            {
                resolvedRoleName = user.Employee.Designation.Role.Name;
                resolvedIsAdmin = user.Employee.Designation.Role.IsAdmin;
            }

            var token = GenerateJwtToken(user, resolvedRoleName, resolvedIsAdmin);
            _logger.LogInformation("User {Username} logged in successfully", user.UserName);

            await _userRepository.UpdateUser(user);

            var expireHours = Convert.ToInt32(_configuration["Jwt:ExpirationHours"]);
            return new LoginResponseDto
            {
                Token = token,
                UserId = user.ID,
                EmployeeId = user.EmployeeID,

                UserName = user.UserName,
                Email = user.EmailId!,
                Role = resolvedRoleName!,
                IsAdmin = resolvedIsAdmin,

                AccountStatus = string.IsNullOrEmpty(user.AccountStatus) ? user.IsActive ? "Active" : "In-Active" : user.AccountStatus,
                LastLoginDate = user.LastLoginDate,
                FailedLoginAttempts = user.FailedLoginAttempts,

                SessionTimeout = user.SessionTimeout,
                TwoFactorEnabled = user.TwoFactorEnabled,
                AutoLockAfterAttempts = user.AutoLockAfterAttempts,
                UnlockMethod = string.IsNullOrEmpty(user.UnlockMethod) ? "Admin" : user.UnlockMethod,

                AllowRemoteLogin = user.RemoteLogin,
                IpRestriction = user.IpRestriction,
                WorkingHours = user.WorkingHours,

                ExpiresInSeconds = expireHours * 60 * 60,
                ProfileImagePath = user.Employee?.ProfileImagePath
            };
        }

        public async Task<object> GetRefreshToken()
        {
            //var oldToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            //var oldToken = loggedInUserDTO?.Email;
            //if (IsTokenExpired(oldToken))
            //{
            //    throw new UnauthorizedAccessException("Token has expired. Please login again.");
            //}
            var loggedInUserDTO = GetCurrentUser();
            if (loggedInUserDTO != null)
            {
                var user = await _userRepository.GetUserByEmail(loggedInUserDTO.Email);
                if (user == null)
                {
                    throw new UnauthorizedAccessException($"User not found: {loggedInUserDTO.Email}");
                }

                var resolvedRoleName = user.RoleName;
                var resolvedIsAdmin = user.IsAdmin;
                if (user.Employee?.Designation?.Role != null)
                {
                    resolvedRoleName = user.Employee.Designation.Role.Name;
                    resolvedIsAdmin = user.Employee.Designation.Role.IsAdmin;
                }

                var token = GenerateJwtToken(user, resolvedRoleName, resolvedIsAdmin);

                var expireHours = Convert.ToInt32(_configuration["Jwt:ExpirationHours"]);
                var responseObject = new
                {
                    token = token,
                    name = user.UserName,
                    email = user.EmailId,
                    role = resolvedRoleName,
                    expiresInSecond = expireHours * 60 * 60,
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

        private string GenerateJwtToken(UserMaster user, string? resolvedRoleName = null, bool resolvedIsAdmin = false)
        {
            var roleName = resolvedRoleName ?? user.RoleName ?? string.Empty;
            var expireHours = Convert.ToInt32(_configuration["Jwt:ExpirationHours"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = string.IsNullOrWhiteSpace(_jwtSecret) ? Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]) : Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim("IsAdmin", resolvedIsAdmin.ToString()),
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

        public string GetHashedPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }
        private bool IsRemoteRequest(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;

            if (remoteIp is null)
                return false;

            return !IPAddress.IsLoopback(remoteIp);
        }


        private bool IsIpAllowed(string? ipRestriction, HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(ipRestriction))
                return true;

            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp is null)
                return false;

            var rules = ipRestriction
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var rule in rules)
            {
                if (IsIpMatch(remoteIp, rule))
                    return true;
            }

            return false;
        }
        private bool IsIpMatch(IPAddress clientIp, string rule)
        {
            if (rule.Contains('/'))
                return IsInCidrRange(clientIp, rule);

            return IPAddress.TryParse(rule, out var allowedIp) &&
                   allowedIp.Equals(clientIp);
        }
        private bool IsInCidrRange(IPAddress address, string cidr)
        {
            var parts = cidr.Split('/');
            if (parts.Length != 2)
                return false;

            if (!IPAddress.TryParse(parts[0], out var baseIp))
                return false;

            if (!int.TryParse(parts[1], out var prefixLength))
                return false;

            var addressBytes = address.GetAddressBytes();
            var baseBytes = baseIp.GetAddressBytes();

            if (addressBytes.Length != baseBytes.Length)
                return false;

            var maskBits = prefixLength;

            for (int i = 0; i < addressBytes.Length && maskBits > 0; i++)
            {
                var mask = maskBits >= 8 ? 255 : (byte)(~(255 >> maskBits));
                if ((addressBytes[i] & mask) != (baseBytes[i] & mask))
                    return false;

                maskBits -= 8;
            }

            return true;
        }


        private bool IsWithinWorkingHours(string? workingHours, DateTimeOffset localTime)
        {
            if (string.IsNullOrWhiteSpace(workingHours))
                return true;

            var parts = workingHours.Split('-', StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
                return false;

            if (!TimeSpan.TryParse(parts[0], out var start))
                return false;

            if (!TimeSpan.TryParse(parts[1], out var end))
                return false;

            var now = localTime.TimeOfDay;

            return now >= start && now <= end;
        }

        private LoggedInUserDTO? GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
                return null;

            return new LoggedInUserDTO
            {
                UserId = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Name = user.FindFirst(ClaimTypes.Name)?.Value ?? "",
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "",
                Role = user.FindFirst(ClaimTypes.Role)?.Value,
                EmployeeID = long.Parse(user.FindFirst("EmployeeID")?.Value ?? "0"),
                CompanyCode = user.FindFirst("CompanyCode")?.Value
            };
        }


    }
}
