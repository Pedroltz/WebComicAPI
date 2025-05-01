using System.IO;

namespace WebComicAPI.Helpers
{
    public static class FileHelper
    {
        public static void CreateMangaFolder(int mangaId)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens", $"manga_{mangaId}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CreateChapterFolder(int mangaId, int chapterId)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens", $"manga_{mangaId}", $"capitulo_{chapterId}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
