using AutoMapper;
using EntityLayer.DTOs;
using EntityLayer.Entities;
using EntityLayer.Entities.DTOs;

namespace WebAPİ.Utulities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookDTO, Book>();
            CreateMap<Book, BookDTO>();
            CreateMap<UserRegisterationDTO, User>();
        }
    }
}
