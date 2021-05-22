using AutoMapper;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Domain.Products.Handlers;

namespace TASI.Backend.Infrastructure.Configs
{
    public class ProductDomainMapperProfile : Profile
    {
        public ProductDomainMapperProfile()
        {
            CreateMap<CreateProductCommand, Product>();
        }
    }
}
