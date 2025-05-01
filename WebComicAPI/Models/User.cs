using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebComicAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } 

        [Required]
        public string Nickname { get; set; }

        public ICollection<MyList> MyLists { get; set; }
    }
}
