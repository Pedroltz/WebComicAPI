using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebComicAPI.Models
{
    public class MyList
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int MangaId { get; set; }
        public Manga Manga { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
