using LibrarySystemWebAPI.Data;
using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibrarySystemWebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        public AuthService(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }
        public async Task<(bool Success, string? Token, string? Role, string? ErrorMessage)> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(username);
            if(user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (false, null, null, "Invalid username or password.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role)
            };
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var duration = double.Parse(jwtSection["DurationInMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (true, tokenString, user.Role, null);

        }

        public async Task<(bool Success, string? ErrorMessage)> SignUpAsync(string username, string password)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(username);
            if(existingUser != null)
            {
                return (false, "Username already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var newUser = new User
            {
                UserName = username,
                PasswordHash = hashedPassword,
                Role = "User" // Default role
            };
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveAsync();
            return (true, null);

        }
    }
}
