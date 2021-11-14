using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Products.Handlers
{
    public class CreateProductCommand : IRequest<IActionResult>
    {
        [Required]
        [StringLength(43, MinimumLength = 5)]
        public string Barcode { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [EnumDataType(typeof(QuantityUnit))]
        public QuantityUnit Unit { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        public bool CanManufacture { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, IActionResult>
    {
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger, TasiContext context,
            IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Products.AnyAsync(x =>
                x.Name.ToLower() == request.Name.ToLower() ||
                x.Barcode.ToLower() == request.Barcode.ToLower(), cancellationToken))
            {
                return new ConflictObjectResult(new ErrorModel("Nama barang atau barcode sudah ada sebelumnya",
                    ErrorCodes.ModelValidation));
            }

            var product = _mapper.Map<Product>(request);
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created product {0} with ID {1}", product.Name, product.ProductId);
            return new JsonResult(product);
        }
    }
}
