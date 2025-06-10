using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebComicAPI.Models.DTOs
{
    public class MangaRequestDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public IFormFile Cover { get; set; }
        public List<int> GenreIds { get; set; } = new();
    }
}
