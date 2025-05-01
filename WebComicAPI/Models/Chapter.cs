// Models/Chapter.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebComicAPI.Models
{
    public class Chapter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MangaId { get; set; }
        public Manga Manga { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public ICollection<Page> Pages { get; set; }
    }
}
