using Microsoft.AspNetCore.Mvc;
using WebComicAPI.Models;
using WebComicAPI.Models.DTOs;
using WebComicAPI.Data;
using FirebaseAdmin.Auth;
using System.Text.Json;
using System.Net.Http.Headers;

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
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequestDTO login)
        {
            try
            {
                var idToken = await LoginFirebase(login.Email, login.Password);
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                var admin = _context.Admins.FirstOrDefault(a => a.Email == login.Email);
                if (admin == null)
                    return Unauthorized("Usuário autenticado no Firebase, mas não está cadastrado como Admin.");

                return Ok(new LoginResponseDTO
                {
                    Token = idToken,
                    Role = "Admin"
                });
            }
            catch (Exception ex)
            {
                return Unauthorized($"Erro: {ex.Message}");
            }
        }

        private async Task<string> LoginFirebase(string email, string password)
        {
            var apiKey = _config["Firebase:ApiKey"];
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}",
                payload
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception("Credenciais inválidas no Firebase.");

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("idToken").GetString();
        }
    }
}
