using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebComicAPI.Data;
using WebComicAPI.Models;

namespace WebComicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerHighlightController : ControllerBase
    {
        private readonly WebComicContext _context;

        public BannerHighlightController(WebComicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BannerHighlight>>> GetBannerHighlights()
        {
            return await _context.BannerHighlights.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BannerHighlight>> GetBannerHighlight(int id)
        {
            var bannerHighlight = await _context.BannerHighlights.FindAsync(id);
            if (bannerHighlight == null) return NotFound();
            return bannerHighlight;
        }

        [HttpPost]
        public async Task<ActionResult<BannerHighlight>> PostBannerHighlight(BannerHighlight bannerHighlight)
        {
            _context.BannerHighlights.Add(bannerHighlight);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBannerHighlight), new { id = bannerHighlight.Id }, bannerHighlight);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBannerHighlight(int id, BannerHighlight bannerHighlight)
        {
            if (id != bannerHighlight.Id) return BadRequest();
            _context.Entry(bannerHighlight).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { if (!BannerHighlightExists(id)) return NotFound(); else throw; }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBannerHighlight(int id)
        {
            var bannerHighlight = await _context.BannerHighlights.FindAsync(id);
            if (bannerHighlight == null) return NotFound();
            _context.BannerHighlights.Remove(bannerHighlight);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BannerHighlightExists(int id) => _context.BannerHighlights.Any(e => e.Id == id);
    }
}
