using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Manufacture.Dtos;
using TASI.Backend.Domain.Manufacture.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Manufacture.Handlers
{
    public class GetManufactureJobsCommand : IRequest<IActionResult>
    {
        public int Page { get; set; } = 0;
        public int Limit { get; set; } = 10;
        public ManufactureStatusCode? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ProductId { get; set; }
    }

    public class GetManufactureJobsCommandHandler : IRequestHandler<GetManufactureJobsCommand, IActionResult>
    {
        private readonly ILogger<GetManufactureJobsCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public GetManufactureJobsCommandHandler(ILogger<GetManufactureJobsCommandHandler> logger, TasiContext context,
            IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(GetManufactureJobsCommand request, CancellationToken cancellationToken)
        {
            var skipCount = request.Page * request.Limit;
            var query = _context.Manufacture
                .Include(x => x.StatusHistory)
                .Include(x => x.Product)
                .AsQueryable();

            // filter by product
            if (request.ProductId != null)
            {
                var supplier = await _context.Products.FindAsync(request.ProductId);
                if (supplier == null)
                {
                    return new NotFoundObjectResult(new ErrorModel("Produk manufaktur tidak ditemukan",
                        ErrorCodes.NotFound));
                }

                query = query.Where(x => x.Product.ProductId == request.ProductId);
            }

            // filter by latest status
            if (request.Status != null)
            {
                query = query.Where(x => x.StatusHistory.Last().Code == request.Status);
            }

            // filter order created date
            if (request.StartDate != null)
            {
                query = query.Where(x => x.StatusHistory.First().ModifiedDate >= request.StartDate);
            }

            // filter order latest date
            if (request.EndDate != null)
            {
                query = query.Where(x => x.StatusHistory.Last().ModifiedDate <= request.EndDate);
            }

            // project data
            var result = await query.Skip(skipCount).Take(request.Limit).ToListAsync(cancellationToken);
            var totalRecords = await query.CountAsync(cancellationToken);
            var resultDto = _mapper.Map<IList<SimpleManufactureJobDto>>(result);

            _logger.LogInformation("Found orders, result: {0} records", totalRecords);
            return new JsonResult(
                new Pagination<SimpleManufactureJobDto>(request.Page, result.Count, totalRecords, resultDto));
        }
    }
}
