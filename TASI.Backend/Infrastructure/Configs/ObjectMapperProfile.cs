using AutoMapper;
using TASI.Backend.Domain.Users.Dto;
using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Infrastructure.Configs
{
    public class ObjectMapperProfile : Profile
    {
        public ObjectMapperProfile()
        {
            CreateMap<User, UserProfileDto>();
            CreateMap<CreateUserLoginDto, User>();
        }
    }
}
