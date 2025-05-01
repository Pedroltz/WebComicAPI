using System.ComponentModel.DataAnnotations;

namespace WebComicAPI.Models
{
    public class Page
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }

        public int Number { get; set; }

        public byte[] Image { get; set; }
    }
}
