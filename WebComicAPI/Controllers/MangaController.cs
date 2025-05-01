using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebComicAPI.Data;
using WebComicAPI.Models;
using WebComicAPI.Helpers;

namespace WebComicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangaController : ControllerBase
    {
        private readonly WebComicContext _context;

        public MangaController(WebComicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manga>>> GetMangas()
        {
            return await _context.Mangas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Manga>> GetManga(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null) return NotFound();
            return manga;
        }

        [HttpPost]
        public async Task<ActionResult<Manga>> PostManga(Manga manga)
        {
            _context.Mangas.Add(manga);
            await _context.SaveChangesAsync();

            // CRIAÇÃO AUTOMÁTICA DA PASTA
            FileHelper.CreateMangaFolder(manga.Id);

            return CreatedAtAction(nameof(GetManga), new { id = manga.Id }, manga);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutManga(int id, Manga manga)
        {
            if (id != manga.Id) return BadRequest();
            _context.Entry(manga).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MangaExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManga(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null) return NotFound();

            _context.Mangas.Remove(manga);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MangaExists(int id)
        {
            return _context.Mangas.Any(e => e.Id == id);
        }
    }
}
