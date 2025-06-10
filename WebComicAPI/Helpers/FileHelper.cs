using System.IO;
using Microsoft.AspNetCore.Http;

namespace WebComicAPI.Helpers
{
    public static class FileHelper
    {
        public static void EnsureCoverFolder()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "covers");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string SaveCover(IFormFile file, int mangaId)
        {
            EnsureCoverFolder();
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"manga_{mangaId}{ext}";
            var path = Path.Combine("wwwroot", "images", "covers", fileName);
            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);
            return Path.Combine("images", "covers", fileName).Replace("\\", "/");
        }

        public static string SavePage(IFormFile file, int mangaId, int chapterId, int number)
        {
            var folder = Path.Combine("wwwroot", "images", "pages", $"manga_{mangaId}", $"chapter_{chapterId}");
            Directory.CreateDirectory(folder);
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"page_{number}{ext}";
            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);
            return Path.Combine("images", "pages", $"manga_{mangaId}", $"chapter_{chapterId}", fileName).Replace("\\", "/");
        }
    }
}
