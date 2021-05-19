using AutoMapper;
using TASI.Backend.Domain.Users.Dto;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Domain.Users.Handlers;

namespace TASI.Backend.Infrastructure.Configs
{
    public class UserDomainMapperProfile : Profile
    {
        public UserDomainMapperProfile()
        {
            CreateMap<User, UserProfileDto>();

            CreateMap<CreateUserCommand, User>();
            CreateMap<CreateUserLoginDto, User>();
        }
    }
}
