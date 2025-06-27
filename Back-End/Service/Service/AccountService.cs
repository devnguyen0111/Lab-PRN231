using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Repositorys.IRepositories;
using Services.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUOW _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly string _userRole = "User";
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Role> _roleRepository;

        public AccountService(IUOW unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _accountRepository = _unitOfWork.GetRepository<Account>();
            _roleRepository = _unitOfWork.GetRepository<Role>();
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _accountRepository.Entities
                .Include(a => a.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? _userRole)
            };

            var token = GenerateJwtToken(claims);
            return token;
        }

        public async Task RegisterAsync(string username, string password, string email)
        {
            if (await _accountRepository.Entities.AnyAsync(u => u.AccountName == username))
                throw new InvalidOperationException("Username is already taken");

            if (await _accountRepository.Entities.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException("Email is already registered");

            var userRole = await _roleRepository.Entities
                .FirstOrDefaultAsync(r => r.RoleName == _userRole)
                ?? throw new InvalidOperationException("User role not found");

            var user = new Account
            {
                AccountName = username,
                Password = password,
                Email = email,
                RoleId = userRole.RoleId
            };

            await _accountRepository.InsertAsync(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return !await _accountRepository.Entities.AnyAsync(u => u.AccountName == username);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _accountRepository.Entities.AnyAsync(u => u.Email == email);
        }

        public Task LogoutAsync()
        {
            // Since JWT is stateless, we don't need to do anything server-side for logout
            return Task.CompletedTask;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            // This would typically use IHttpContextAccessor to get the current user's ID from claims
            // For now, this is just a placeholder
            throw new NotImplementedException();
        }

        public async Task<string> GetCurrentUserNameAsync()
        {
            // This would typically use IHttpContextAccessor to get the current username from claims
            // For now, this is just a placeholder
            throw new NotImplementedException();
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var userId = await GetCurrentUserIdAsync();
            var user = await _accountRepository.GetByIdAsync(int.Parse(userId));

            if (user == null || user.Password != currentPassword)
                return false;

            user.Password = newPassword;
            _accountRepository.Update(user);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _accountRepository.Entities.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return false;

            user.Password = newPassword;
            _accountRepository.Update(user);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private string GenerateJwtToken(List<Claim> claims)
        {
            var key = _configuration["JwtSettings:Key"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT Key is not configured");
            if (string.IsNullOrEmpty(issuer))
                throw new InvalidOperationException("JWT Issuer is not configured");
            if (string.IsNullOrEmpty(audience))
                throw new InvalidOperationException("JWT Audience is not configured");
            if (expiryMinutes <= 0)
                expiryMinutes = 60; // Default to 60 minutes if not configured

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
