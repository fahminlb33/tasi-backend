using AutoMapper;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Domain.Suppliers.Handlers;

namespace TASI.Backend.Infrastructure.Configs
{
    public class SupplierDomainMapperProfile : Profile
    {
        public SupplierDomainMapperProfile()
        {
            CreateMap<CreateSupplierCommand, Supplier>();
        }
    }
}
