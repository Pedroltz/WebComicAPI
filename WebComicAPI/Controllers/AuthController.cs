using Microsoft.AspNetCore.Mvc;
using WebComicAPI.Models;
using WebComicAPI.Models.DTOs;
using WebComicAPI.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

namespace WebComicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly WebComicContext _context;
        private readonly IConfiguration _config;

        public AuthController(WebComicContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost("login-admin")]
        public IActionResult LoginAdmin([FromBody] LoginRequestDTO login)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == login.Email && a.Password == login.Password);
            if (admin == null)
                return Unauthorized("Credenciais inválidas");

            var token = GenerateJwtToken(admin.Email, "Admin");
            return Ok(new LoginResponseDTO { Token = token, Role = "Admin" });
        }

        [HttpPost("login-user")]
        public IActionResult LoginUser([FromBody] LoginRequestDTO login)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (user == null)
                return Unauthorized("Credenciais inválidas");

            var token = GenerateJwtToken(user.Email, "User");
            return Ok(new LoginResponseDTO { Token = token, Role = "User" });
        }

        private string GenerateJwtToken(string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role)
    };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
