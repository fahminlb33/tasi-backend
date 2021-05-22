using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Suppliers.Handlers
{
    public class GetSuppliersCommand : IRequest<IActionResult>
    {
        public int Page { get; set; }
        public int Limit { get; set; } = 10;
        public string? Query { get; set; }
    }

    public class GetSuppliersCommandHandler : IRequestHandler<GetSuppliersCommand, IActionResult>
    {
        private readonly ILogger<GetSuppliersCommandHandler> _logger;
        private readonly TasiContext _context;

        public GetSuppliersCommandHandler(ILogger<GetSuppliersCommandHandler> logger, TasiContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Handle(GetSuppliersCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Finding suppliers with query {0}", request.Query);

            var skipCount = request.Page * request.Limit;
            var query = _context.Suppliers.AsQueryable();
            if (request.Query != null)
            {
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{request.Query}%"))
                    .Where(x => EF.Functions.Like(x.Address, $"%{request.Query}%"));
            }

            var result = await query.Skip(skipCount).Take(request.Limit).ToListAsync(cancellationToken);
            var totalRecords = await query.CountAsync(cancellationToken);

            return new JsonResult(new Pagination<Supplier>(request.Page, result.Count, totalRecords, result));
        }
    }

}
