#nullable enable

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Products.Handlers
{
    public class GetProductsCommand : IRequest<IActionResult>
    {
        public int Page { get; set; }
        public int Limit { get; set; } = 10;
        public string? Query { get; set; }
    }

    public class GetProductsCommandHandler : IRequestHandler<GetProductsCommand, IActionResult>
    {
        private readonly ILogger<GetProductsCommandHandler> _logger;
        private readonly TasiContext _context;

        public GetProductsCommandHandler(ILogger<GetProductsCommandHandler> logger, TasiContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Handle(GetProductsCommand request, CancellationToken cancellationToken)
        {
            var skipCount = request.Page * request.Limit;
            var query = _context.Products.AsQueryable();
            if (request.Query != null)
            {
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{request.Query}%"))
                    .Where(x => EF.Functions.Like(x.Barcode, $"%{request.Query}%"));

                if (int.TryParse(request.Query, out int queryInt))
                {
                    query = query.Where(x => x.Stock == queryInt);
                }
            }

            var result = await query.Skip(skipCount).Take(request.Limit).ToListAsync(cancellationToken);
            var totalRecords = await query.CountAsync(cancellationToken);

            _logger.LogInformation("Find product data with query {0}, result: {1} records", request.Query,
                totalRecords);
            return new JsonResult(new Pagination<Product>(request.Page, result.Count, totalRecords, result));
        }
    }
}
