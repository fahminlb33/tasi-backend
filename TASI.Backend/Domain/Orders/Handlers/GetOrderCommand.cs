using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Orders.Handlers
{
    public class GetOrderCommand : IRequest<IActionResult>
    {
        public int OrderId { get; set; }
    }

    public class GetOrderCommandHandler : IRequestHandler<GetOrderCommand, IActionResult>
    {
        private readonly ILogger<GetOrderCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public GetOrderCommandHandler(ILogger<GetOrderCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(GetOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .Include(x => x.StatusHistory)
                .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);
            if (order == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            _logger.LogInformation("Found order for ID {0}", order.OrderId);
            return new JsonResult(_mapper.Map<OrderDto>(order));
        }
    }
}
