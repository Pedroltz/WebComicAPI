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
    public class ChapterController : ControllerBase
    {
        private readonly WebComicContext _context;

        public ChapterController(WebComicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chapter>>> GetChapters()
        {
            return await _context.Chapters.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Chapter>> GetChapter(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null) return NotFound();
            return chapter;
        }

        [HttpPost]
        public async Task<ActionResult<Chapter>> PostChapter([FromForm] ChapterRequestDTO dto)
        {
            if (!await _context.Mangas.AnyAsync(m => m.Id == dto.MangaId))
                return BadRequest("Manga not found.");

            var chapter = new Chapter { MangaId = dto.MangaId, Number = dto.Number, Title = dto.Title };
            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            if (dto.Pages != null)
            {
                int number = 1;
                foreach (var file in dto.Pages)
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext))
                        return BadRequest("Invalid page file.");
                    var path = FileHelper.SavePage(file, dto.MangaId, chapter.Id, number);
                    _context.Pages.Add(new Page { ChapterId = chapter.Id, Number = number, ImagePath = path });
                    number++;
                }
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, chapter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutChapter(int id, [FromForm] ChapterRequestDTO dto)
        {
            var chapter = await _context.Chapters.Include(c => c.Pages).FirstOrDefaultAsync(c => c.Id == id);
            if (chapter == null) return NotFound();

            chapter.Number = dto.Number;
            chapter.Title = dto.Title;

            if (dto.Pages != null && dto.Pages.Count > 0)
            {
                _context.Pages.RemoveRange(chapter.Pages);
                int number = 1;
                foreach (var file in dto.Pages)
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext))
                        return BadRequest("Invalid page file.");
                    var path = FileHelper.SavePage(file, chapter.MangaId, chapter.Id, number);
                    _context.Pages.Add(new Page { ChapterId = chapter.Id, Number = number, ImagePath = path });
                    number++;
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null) return NotFound();

            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ChapterExists(int id)
        {
            return _context.Chapters.Any(e => e.Id == id);
        }
    }
}
