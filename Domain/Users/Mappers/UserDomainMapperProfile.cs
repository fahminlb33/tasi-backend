using AutoMapper;
using TASI.Backend.Domain.Users.Dtos;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Domain.Users.Handlers;

namespace TASI.Backend.Domain.Users.Mappers
{
    public class UserDomainMapperProfile : Profile
    {
        public UserDomainMapperProfile()
        {
            CreateMap<User, UserProfileDto>();

            CreateMap<CreateUserCommand, User>()
                .ForMember(x => x.Password, options => options.AddTransform(x => BCrypt.Net.BCrypt.HashPassword(x)));
            CreateMap<EditUserDto, User>()
                .ForMember(x => x.UserId, options => options.Ignore())
                .ForAllOtherMembers(options =>
                    options.Condition((_, _, member) => member != null));
        }
    }
}
