using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Suppliers.Handlers
{
    public class DeleteSupplierCommand : IRequest<IActionResult>
    {
        public int SupplierId { get; set; }
    }

    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, IActionResult>
    {
        private readonly ILogger<DeleteSupplierCommandHandler> _logger;
        private readonly TasiContext _context;

        public DeleteSupplierCommandHandler(ILogger<DeleteSupplierCommandHandler> logger, TasiContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Deleting supplier for ID {0}", request.SupplierId);

            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            if (supplier == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync(cancellationToken);

            return new OkResult();
        }
    }
}
