﻿using System.Linq;
using AutoMapper;
using TASI.Backend.Domain.Manufacture.Dtos;
using TASI.Backend.Domain.Manufacture.Entities;

namespace TASI.Backend.Domain.Manufacture.Mappers
{
    public class ManufactureDomainMapperProfile : Profile
    {
        public ManufactureDomainMapperProfile()
        {
            CreateMap<ManufactureJob, ManufactureJobDto>()
                .ForMember(x => x.ProductId,
                    options => options.MapFrom(p => p.Product.ProductId))
                .ForMember(x => x.LastStatus,
                    options => options.MapFrom(p => p.StatusHistory.Last()));
            
            CreateMap<ManufactureStatusDto, ManufactureStatus>();
            CreateMap<ManufactureStatus, ManufactureStatusDto>();
            
            CreateMap<ManufactureMaterial, ManufactureMaterialDto>()
                .ForMember(x => x.ProductId,
                    options => options.MapFrom(p => p.Product.ProductId));
        }
    }
}
