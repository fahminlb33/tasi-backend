#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Helpers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Orders.Handlers
{
    public class GetOrdersCommand : IRequest<IActionResult>
    {
        public int Page { get; set; } = 0;
        public int Limit { get; set; } = 10;
        public OrderType? Type { get; set; }
        public OrderStatusCode? Status { get; set; }
        public DateTime? StartDate { get;set; }
        public DateTime? EndDate { get;set; }
        public int? SupplierId { get; set; }
        public int? UserId { get; set; }
    }

    public class GetOrdersCommandHandler : IRequestHandler<GetOrdersCommand, IActionResult>
    {
        private readonly ILogger<GetOrdersCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public GetOrdersCommandHandler(ILogger<GetOrdersCommandHandler> logger, IHttpContextAccessor httpContext, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _httpContext = httpContext;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(GetOrdersCommand request, CancellationToken cancellationToken)
        {
            var skipCount = request.Page * request.Limit;
            var query = _context.Orders
                .Include(x => x.StatusHistory)
                .AsQueryable();

            var loggedUser = await _context.Users.FindAsync(_httpContext.GetUserIdFromClaim());
            if (loggedUser.Role == UserRole.Customer)
            {
                // if user is logged in as customer, only show their orders
                query = query.Where(x => x.PicUser.UserId == loggedUser.UserId);
            }
            else
            {
                // if user is logged in as other than customer, filter the order by user
                if (request.UserId != null)
                {
                    var user = await _context.Users.FindAsync(request.UserId);
                    if (user == null)
                    {
                        return new NotFoundObjectResult(new ErrorModel("User tidak ditemukan", ErrorCodes.NotFound));
                    }

                    query = query.Where(x => x.PicUser.UserId == request.UserId);
                }
            }

            // filter by supplier
            if (request.SupplierId != null)
            {
                var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
                if (supplier == null)
                {
                    return new NotFoundObjectResult(new ErrorModel("Supplier tidak ditemukan", ErrorCodes.NotFound));
                }

                query = query.Where(x => x.Supplier.SupplierId == request.SupplierId);
            }

            // filter by order type
            if (request.Type != null)
            {
                query = query.Where(x => x.Type == request.Type);
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

            var result = await query.Skip(skipCount).Take(request.Limit).ToListAsync(cancellationToken);
            var totalRecords = await query.CountAsync(cancellationToken);
            var resultDto = _mapper.Map<IList<SimpleOrderDto>>(result);

            _logger.LogInformation("Found orders, result: {0} records", totalRecords);
            return new JsonResult(new Pagination<SimpleOrderDto>(request.Page, result.Count, totalRecords, resultDto));
        }
    }
    
}
