using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Suppliers.Handlers
{
    public class GetSupplierCommand : IRequest<IActionResult>
    {
        public int SupplierId { get; set; }
    }

    public class GetSupplierCommandHandler : IRequestHandler<GetSupplierCommand, IActionResult>
    {
        private readonly ILogger<GetSupplierCommandHandler> _logger;
        private readonly TasiContext _context;

        public GetSupplierCommandHandler(ILogger<GetSupplierCommandHandler> logger, TasiContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Handle(GetSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            if (supplier == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            _logger.LogInformation("Found supplier with ID {0}", request.SupplierId);
            return new JsonResult(supplier);
        }
    }
}
