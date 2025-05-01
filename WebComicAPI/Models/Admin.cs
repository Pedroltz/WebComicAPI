using System.ComponentModel.DataAnnotations;

namespace WebComicAPI.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
