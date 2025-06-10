using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebComicAPI.Models
{
    public class Manga
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string CoverPath { get; set; }

        public ICollection<Chapter> Chapters { get; set; }

        public ICollection<MangaGenre> MangaGenres { get; set; }
    }
}
