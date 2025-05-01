using System.ComponentModel.DataAnnotations;

namespace WebComicAPI.Models
{
    public class BannerHighlight
    {
        [Key]
        public int Id { get; set; }

        public byte[] Image { get; set; }

        public int? MangaId { get; set; }
        public Manga Manga { get; set; }

        public string Link { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }
    }
}
