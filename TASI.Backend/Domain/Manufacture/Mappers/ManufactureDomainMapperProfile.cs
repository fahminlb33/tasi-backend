using System.Linq;
using AutoMapper;
using TASI.Backend.Domain.Manufacture.Dtos;
using TASI.Backend.Domain.Manufacture.Entities;

namespace TASI.Backend.Domain.Manufacture.Mappers
{
    public class ManufactureDomainMapperProfile : Profile
    {
        public ManufactureDomainMapperProfile()
        {
            CreateMap<ManufactureJob, ManufactureJobDto>();
            CreateMap<ManufactureJob, SimpleManufactureJobDto>()
                .ForMember(x => x.ProductId,
                    options => options.MapFrom(p => p.Product.ProductId))
                .ForMember(x => x.LastStatus,
                    options => options.MapFrom(p => p.StatusHistory.Last()));

            CreateMap<ProcessManufactureDto, ManufactureStatus>();
            CreateMap<ManufactureStatusDto, ManufactureStatus>();
            CreateMap<ManufactureStatus, ManufactureStatusDto>();

            CreateMap<ManufactureMaterial, ManufactureMaterialDto>()
                .ForMember(x => x.ProductId,
                    options => options.MapFrom(p => p.Product.ProductId));
        }
    }
}
