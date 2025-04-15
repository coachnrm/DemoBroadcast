using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.DTO;

namespace BackEnd.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController
        (UserManager<AppUser> userManager, IConfiguration config) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            var user = new AppUser
            {
                UserName = register.UserName,
                Email = register.Email,
                Name = register.Name
            };
            var result = await userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
                return Ok("succeeded");
            else 
                return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var user = await userManager.FindByEmailAsync(login.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            bool result = await userManager.CheckPasswordAsync(user, login.Password);
            if (!result) return Unauthorized("Invalid credentials");

            string token = string.Empty;

            if (login.Email.StartsWith("admin"))
                token = GenerateJwtToken(user, "Admin");
            else
                token = GenerateJwtToken(user, "User");
            return Ok(token);
        }

        private string GenerateJwtToken(AppUser user, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.Name!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!)
            };
            var token = new JwtSecurityToken(
                            issuer: config["Jwt:Issuer"],
                            audience: config["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(2),
                            signingCredentials: creds
                        );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
