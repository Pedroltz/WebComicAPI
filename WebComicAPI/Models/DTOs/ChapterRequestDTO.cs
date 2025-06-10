using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebComicAPI.Models.DTOs
{
    public class ChapterRequestDTO
    {
        [Required]
        public int MangaId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public IFormFileCollection Pages { get; set; }
    }
}
