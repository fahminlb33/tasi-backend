using AutoMapper;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Domain.Suppliers.Handlers;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Domain.Users.Handlers;

namespace TASI.Backend.Infrastructure.Configs
{
    public class SupplierDomainMapperProfile : Profile
    {
        public SupplierDomainMapperProfile()
        {
            CreateMap<CreateSupplierCommand, Supplier>();
            CreateMap<EditSupplierCommandBody, Supplier>()
                .ForMember(x => x.SupplierId, options => options.Ignore())
                .ForAllOtherMembers(options =>
                    options.Condition((_, _, member) => member != null));
        }
    }
}
