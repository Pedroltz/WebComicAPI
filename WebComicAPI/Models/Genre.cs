using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebComicAPI.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<MangaGenre> MangaGenres { get; set; }
    }
}
