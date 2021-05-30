using AutoMapper;
using TASI.Backend.Domain.Products.Dtos;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Domain.Products.Handlers;

namespace TASI.Backend.Domain.Products.Mappers
{
    public class ProductDomainMapperProfile : Profile
    {
        public ProductDomainMapperProfile()
        {
            CreateMap<CreateProductCommand, Product>();
            CreateMap<EditProductDto, Product>()
                .ForMember(x => x.ProductId, options => options.Ignore())
                .ForAllOtherMembers(options =>
                    options.Condition((_, _, member) => member != null));
        }
    }
}
