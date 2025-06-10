using AutoMapper;
using WebComicAPI.Models;
using WebComicAPI.Models.DTOs;

namespace WebComicAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MangaRequestDTO, Manga>();
            CreateMap<ChapterRequestDTO, Chapter>();
        }
    }
}
