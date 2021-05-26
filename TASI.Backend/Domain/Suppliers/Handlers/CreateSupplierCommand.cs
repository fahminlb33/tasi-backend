using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Suppliers.Handlers
{
    public class CreateSupplierCommand : IRequest<IActionResult>
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; }

        [Required]
        public decimal ShippingCost { get; set; }
    }

    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, IActionResult>
    {
        private readonly ILogger<CreateSupplierCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public CreateSupplierCommandHandler(ILogger<CreateSupplierCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Suppliers.AnyAsync(x =>
                x.Name.ToLower() == request.Name.ToLower() , cancellationToken))
            {
                return new ConflictObjectResult(new ErrorModel("Nama supplier sudah ada sebelumnya",
                    ErrorCodes.ModelValidation));
            }

            var supplier = _mapper.Map<Supplier>(request);
            await _context.Suppliers.AddAsync(supplier, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created supplier {0} with ID {1}", supplier.Name, supplier.SupplierId);
            return new JsonResult(await _context.Suppliers.FindAsync(supplier.SupplierId));

        }
    }
}
