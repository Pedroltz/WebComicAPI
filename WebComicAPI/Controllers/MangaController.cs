using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebComicAPI.Data;
using WebComicAPI.Models;
using WebComicAPI.Helpers;
using WebComicAPI.Models.DTOs;
using System.IO;

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
        public async Task<ActionResult<Manga>> PostManga([FromForm] MangaRequestDTO dto)
        {
            var genres = await _context.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();
            if (genres.Count != dto.GenreIds.Count)
                return BadRequest("One or more genres not found.");

            var manga = new Manga { Name = dto.Name, Description = dto.Description, Author = dto.Author };
            _context.Mangas.Add(manga);
            await _context.SaveChangesAsync();

            foreach (var id in dto.GenreIds)
                _context.MangaGenres.Add(new MangaGenre { MangaId = manga.Id, GenreId = id });

            if (dto.Cover != null && dto.Cover.Length > 0)
            {
                var ext = Path.GetExtension(dto.Cover.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext))
                    return BadRequest("Invalid cover file.");
                manga.CoverPath = FileHelper.SaveCover(dto.Cover, manga.Id);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetManga), new { id = manga.Id }, manga);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutManga(int id, [FromForm] MangaRequestDTO dto)
        {
            var manga = await _context.Mangas.Include(m => m.MangaGenres).FirstOrDefaultAsync(m => m.Id == id);
            if (manga == null) return NotFound();

            manga.Name = dto.Name;
            manga.Description = dto.Description;
            manga.Author = dto.Author;

            _context.MangaGenres.RemoveRange(manga.MangaGenres);
            foreach (var gid in dto.GenreIds)
            {
                if (!await _context.Genres.AnyAsync(g => g.Id == gid))
                    return BadRequest($"Genre {gid} not found.");
                _context.MangaGenres.Add(new MangaGenre { MangaId = manga.Id, GenreId = gid });
            }

            if (dto.Cover != null && dto.Cover.Length > 0)
            {
                var ext = Path.GetExtension(dto.Cover.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext))
                    return BadRequest("Invalid cover file.");
                manga.CoverPath = FileHelper.SaveCover(dto.Cover, manga.Id);
            }

            await _context.SaveChangesAsync();
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
