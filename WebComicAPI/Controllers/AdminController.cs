using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebComicAPI.Data;
using WebComicAPI.Models;
using FirebaseAdmin.Auth;

namespace WebComicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly WebComicContext _context;

        public AdminController(WebComicContext context)
        {
            _context = context;
        }

        // GET: api/admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAllAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        // GET: api/admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdminById(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();
            return admin;
        }

        // POST: api/admin
        [HttpPost]
        public async Task<ActionResult<Admin>> CreateAdmin([FromBody] Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAdminById), new { id = admin.Id }, admin);
        }

        // POST: api/admin/create-with-firebase
        [HttpPost("create-with-firebase")]
        public async Task<ActionResult<Admin>> CreateAdminWithFirebase([FromBody] Admin admin)
        {
            try
            {
                // Cria no Firebase Authentication
                var args = new UserRecordArgs
                {
                    Email = admin.Email,
                    Password = admin.Password,
                    EmailVerified = false,
                    Disabled = false
                };

                await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

                // Cria no banco de dados
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAdminById), new { id = admin.Id }, admin);
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest($"Erro ao criar no Firebase: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] Admin admin)
        {
            if (id != admin.Id)
                return BadRequest();

            var existingAdmin = await _context.Admins.FindAsync(id);
            if (existingAdmin == null)
                return NotFound();

            // Verifica se a senha foi alterada
            var senhaFoiAlterada = existingAdmin.Password != admin.Password;

            // Atualiza os dados locais
            existingAdmin.Email = admin.Email;
            existingAdmin.Password = admin.Password;

            try
            {
                _context.Admins.Update(existingAdmin);
                await _context.SaveChangesAsync();

                if (senhaFoiAlterada)
                {
                    var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(admin.Email);
                    var updateRequest = new UserRecordArgs
                    {
                        Uid = firebaseUser.Uid,
                        Password = admin.Password
                    };
                    await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateRequest);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return NotFound();

            try
            {
                // Tenta buscar o usuário no Firebase pelo email
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(admin.Email);
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(user.Uid);
            }
            catch (FirebaseAdmin.Auth.FirebaseAuthException ex)
            {
                // Usuário não encontrado no Firebase não impede exclusão no banco
                Console.WriteLine($"Firebase error: {ex.Message}");
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
