using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebComicAPI.Data;
using WebComicAPI.Models;
using Microsoft.AspNetCore.Http;

namespace WebComicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly WebComicContext _context;

        public PageController(WebComicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Page>>> GetPages()
        {
            return await _context.Pages.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Page>> GetPage(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null) return NotFound();
            return page;
        }

        [HttpPost]
        public async Task<ActionResult<Page>> PostPage(Page page)
        {
            _context.Pages.Add(page);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPage), new { id = page.Id }, page);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPage(int id, Page page)
        {
            if (id != page.Id) return BadRequest();
            _context.Entry(page).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { if (!PageExists(id)) return NotFound(); else throw; }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePage(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null) return NotFound();
            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PageExists(int id) => _context.Pages.Any(e => e.Id == id);

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "pages", $"manga_{mangaId}", $"chapter_{chapterId}");
        [HttpPost("upload/{mangaId}/{chapterId}")]
        public async Task<IActionResult> UploadPageImage(int mangaId, int chapterId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens", $"manga_{mangaId}", $"capitulo_{chapterId}");
            if (!Directory.Exists(folderPath))
                return NotFound("Pasta de capítulo não encontrada.");

            var fileName = Path.GetFileName(file.FileName);
            var savePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "Imagem enviada com sucesso!", fileName = fileName });
        }
    }
}
