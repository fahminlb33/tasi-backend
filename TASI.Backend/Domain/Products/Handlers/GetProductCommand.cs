using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Products.Handlers
{
    public class GetProductCommand : IRequest<IActionResult>
    {
        public int ProductId { get; set; }
    }

    public class GetProductCommandHandler : IRequestHandler<GetProductCommand, IActionResult>
    {
        private readonly ILogger<GetProductCommandHandler> _logger;
        private readonly TasiContext _context;

        public GetProductCommandHandler(ILogger<GetProductCommandHandler> logger, TasiContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Handle(GetProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            _logger.LogInformation("Found product for ID {0}", request.ProductId);
            return new JsonResult(product);
        }
    }
}
