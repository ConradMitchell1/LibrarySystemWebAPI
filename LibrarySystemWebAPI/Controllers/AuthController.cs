using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Models;
using LibrarySystemWebAPI.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystemWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.UserName, request.Password);

            if(!result.Success || result.Token == null)
            {
                return Unauthorized(result.ErrorMessage ?? "Login failed.");
            }

            Response.Cookies.Append("jwt", result.Token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
            });

            return Ok(new { token = result.Token, role = result.Role });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok("Logged out successfully.");
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromForm] UserDTO request)
        {
            var result = await _authService.SignUpAsync(request.UserName, request.Password);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage ?? "Signup failed.");
            }
            return Ok("User created successfully.");
        }
    }
}
