using System.Linq;
using AutoMapper;
using TASI.Backend.Domain.Orders.Dtos;
using TASI.Backend.Domain.Orders.Entities;

namespace TASI.Backend.Domain.Orders.Mappers
{
    public class OrderDomainMapperProfile : Profile
    {
        public OrderDomainMapperProfile()
        {
            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(x => x.ProductId,
                    options => options.MapFrom(p => p.Product.ProductId))
                .ForMember(x => x.ProductName,
                    options => options.MapFrom(p => p.Product.Name));

            CreateMap<OrderStatus, OrderStatusDto>();

            CreateMap<Order, OrderDto>();
            CreateMap<Order, SimpleOrderDto>()
                .ForMember(x => x.SupplierName,
                    options => options.MapFrom(p => p.Supplier.Name))
                .ForMember(x => x.UserFullname,
                    options => options.MapFrom(p => p.PicUser.FullName))
                .ForMember(x => x.LastStatus,
                    options => options.MapFrom(p => p.StatusHistory.Last()));

            CreateMap<ProcessOrderDto, OrderStatus>();
        }
    }
}
