using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Products.Dtos;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Products.Handlers
{
    public class EditProductCommand : IRequest<IActionResult>
    {
        public int ProductId { get; set; }
        public EditProductDto Body { get; set; }
    }

    public class EditProductCommandHandler : IRequestHandler<EditProductCommand, IActionResult>
    {
        private readonly ILogger<EditProductCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public EditProductCommandHandler(ILogger<EditProductCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _context.Products.FindAsync(new object[] {request.ProductId}, cancellationToken);
            if (supplier == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            if (request.Body?.Name != null)
            {
                if (await _context.Products.AnyAsync(x => x.Name.ToLower() == request.Body.Name.ToLower(),
                    cancellationToken))
                {
                    return new ConflictObjectResult(new ErrorModel("Nama sudah digunakan pada produk sebelumnya.",
                        ErrorCodes.DataDuplicated));
                }
            }

            if (request.Body?.Barcode != null)
            {
                if (await _context.Products.AnyAsync(x => x.Barcode.ToLower() == request.Body.Barcode.ToLower(),
                    cancellationToken))
                {
                    return new ConflictObjectResult(new ErrorModel("Barcode sudah digunakan pada produk sebelumnya.",
                        ErrorCodes.DataDuplicated));
                }
            }

            var updatedEntity = _mapper.Map(request.Body, supplier);
            _context.Products.Update(updatedEntity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product with ID {0}", supplier.ProductId);
            return new JsonResult(supplier);
        }
    }
}
