using AutoMapper;
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Domain.Orders.Entities;

namespace TASI.Backend.Domain.Orders.Mappers
{
    public class OrderDomainMapperProfile : Profile
    {
        public OrderDomainMapperProfile()
        {
            CreateMap<OrderDetail, OrderDetailDto>();
            
            CreateMap<OrderStatus, OrderStatusDto>();

            CreateMap<Order, OrderDto>();
            CreateMap<Order, SimpleOrderDto>();

            CreateMap<ProcessOrderDto, OrderStatus>();
        }
    }
}
