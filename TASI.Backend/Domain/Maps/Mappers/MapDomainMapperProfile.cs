using AutoMapper;
using TASI.Backend.Domain.Maps.Dtos;
using TASI.Backend.Infrastructure.Services;

namespace TASI.Backend.Domain.Maps.Mappers
{
    public class MapDomainMapperProfile : Profile
    {
        public MapDomainMapperProfile()
        {
            CreateMap<ReverseGeocodedAddress, LookupAddressDto>()
                .ForMember(x => x.OriginalAddress, 
                    options => options.MapFrom(p => p.Address));
        }
    }
}
