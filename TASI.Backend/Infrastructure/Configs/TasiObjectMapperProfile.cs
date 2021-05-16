using AutoMapper;
using TASI.Backend.Domain.Users.Dto;
using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Infrastructure.Configs
{
    public class TasiObjectMapperProfile : Profile
    {
        public TasiObjectMapperProfile()
        {
            CreateMap<CreateUserLoginDto, Login>();
            CreateMap<CreateUserLoginDto, User>();

            CreateMap<User, EditUserLoginDto>()
                .ForMember(x => x.Username, m => m.MapFrom(y => y.Login.Username));

        }
    }
}
