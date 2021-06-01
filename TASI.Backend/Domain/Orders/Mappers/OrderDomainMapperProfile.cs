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
                    options => options.MapFrom(p => p.Product.ProductId));

            CreateMap<OrderStatus, OrderStatusDto>();

            CreateMap<Order, OrderDto>();
            CreateMap<Order, SimpleOrderDto>()
                .ForMember(x => x.LastStatus,
                    options => options.MapFrom(p => p.StatusHistory.Last()));

            CreateMap<ProcessOrderDto, OrderStatus>();
        }
    }
}
