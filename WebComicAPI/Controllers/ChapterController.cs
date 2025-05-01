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
        public async Task<ActionResult<Chapter>> PostChapter(Chapter chapter)
        {
            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            // CRIAÇÃO AUTOMÁTICA DA PASTA
            FileHelper.CreateChapterFolder(chapter.MangaId, chapter.Id);

            return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, chapter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutChapter(int id, Chapter chapter)
        {
            if (id != chapter.Id) return BadRequest();
            _context.Entry(chapter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChapterExists(id))
                    return NotFound();
                else
                    throw;
            }

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
