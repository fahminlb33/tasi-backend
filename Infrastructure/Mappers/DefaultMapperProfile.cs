using System;
using AutoMapper;

namespace TASI.Backend.Infrastructure.Mappers
{
    public class DefaultMapperProfile : Profile
    {
        public DefaultMapperProfile()
        {
            CreateMap<decimal, decimal>()
                .ConvertUsing(x => Math.Round(x, 2));
            CreateMap<double, double>()
                .ConvertUsing(x => Math.Round(x, 2));
        }
    }
}
