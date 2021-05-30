using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Orders.Handlers
{
    public class ProcessOrderCommand: IRequest<IActionResult>
    {
        public int OrderId { get; set; }
        public ProcessOrderDto Body { get; set; }
    }

    public class ProcessOrderCommandHandler : IRequestHandler<ProcessOrderCommand, IActionResult>
    {
        private readonly ILogger<ProcessOrderCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public ProcessOrderCommandHandler(ILogger<ProcessOrderCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(x => x.StatusHistory)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);
            if (order == null)
            {
                return new NotFoundObjectResult(new ErrorModel("Order tidak ditemukan", ErrorCodes.NotFound));
            }

            _logger.LogDebug("Transitioning order status for order ID {0}", request.OrderId);
            return request.Body.Code switch
            {
                OrderStatusCode.Requested => GetInvalidSequentialProcessResponse(),
                OrderStatusCode.InProcess => await TransitionToInProcess(request, order, cancellationToken),
                OrderStatusCode.Delivery => await TransitionToDelivery(request, order, cancellationToken),
                OrderStatusCode.Completed => await TransitionToCompleted(request, order, cancellationToken),
                OrderStatusCode.Cancelled => await TransitionToCancelled(request, order, cancellationToken),
                _ => new BadRequestObjectResult(new ErrorModel("Status tidak diketahui", ErrorCodes.ModelValidation,
                    request.Body.Code))
            };
        }

        private IActionResult GetInvalidSequentialProcessResponse()
        {
            return new ConflictObjectResult(new ErrorModel("Tidak bisa menggunakan status ini karena status sebelumnya tidak valid",
                ErrorCodes.InvalidSequentialProcess));
        }

        private async Task UpdateOrder(ProcessOrderCommand request, Order order, CancellationToken cancellationToken)
        {
            // append status
            var status = _mapper.Map<OrderStatus>(request.Body);
            status.Order = order;

            order.StatusHistory.Add(status);
            _context.Orders.Update(order);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<IActionResult> TransitionToInProcess(ProcessOrderCommand request, Order order, CancellationToken cancellationToken)
        {
            var latestStatus = order.StatusHistory.OrderBy(x => x.ModifiedDate).Last();
            if (latestStatus.Code != OrderStatusCode.Requested)
            {
                return GetInvalidSequentialProcessResponse();
            }

            // check stock
            if (order.Type == OrderType.Sales && order.OrderDetails.Any(x => x.Product.Quantity - x.Quantity < 0))
            {
                var insufficientProducts = order.OrderDetails
                    .Where(x => x.Product.Quantity - x.Quantity < 0)
                    .Select(x => $"Stok barang {x.Product.Name} ({x.Product.Barcode}) kurang/tidak tersedia");
                return new ConflictObjectResult(new ErrorModel("Ada barang yang tidak tersedia di gudang",
                    ErrorCodes.NotEnoughStock, insufficientProducts));
            }

            // update stock if it's sales
            if (order.Type == OrderType.Sales)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await _context.Products.FindAsync(orderDetail.Product.ProductId);
                    product.Quantity -= orderDetail.Quantity;

                    _context.Products.Update(product);
                }
            }

            // update order
            await UpdateOrder(request, order, cancellationToken);
            return new OkResult();
        }

        private async Task<IActionResult> TransitionToDelivery(ProcessOrderCommand request, Order order, CancellationToken cancellationToken)
        {
            var latestStatus = order.StatusHistory.OrderBy(x => x.ModifiedDate).Last();
            if (latestStatus.Code != OrderStatusCode.InProcess)
            {
                return GetInvalidSequentialProcessResponse();
            }

            // update order
            await UpdateOrder(request, order, cancellationToken);
            return new OkResult();
        }

        private async Task<IActionResult> TransitionToCompleted(ProcessOrderCommand request, Order order, CancellationToken cancellationToken)
        {
            var latestStatus = order.StatusHistory.OrderBy(x => x.ModifiedDate).Last();
            if (latestStatus.Code != OrderStatusCode.Delivery)
            {
                return GetInvalidSequentialProcessResponse();
            }

            // update stock if it's sales
            if (order.Type == OrderType.Supplier)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await _context.Products.FindAsync(orderDetail.Product.ProductId);
                    product.Quantity += orderDetail.Quantity;

                    _context.Products.Update(product);
                }
            }

            // update order
            await UpdateOrder(request, order, cancellationToken);
            return new OkResult();
        }

        private async Task<IActionResult> TransitionToCancelled(ProcessOrderCommand request, Order order, CancellationToken cancellationToken)
        {
            var latestStatus = order.StatusHistory.OrderBy(x => x.ModifiedDate).Last();
            if (latestStatus.Code != OrderStatusCode.InProcess)
            {
                return GetInvalidSequentialProcessResponse();
            }

            // update stock
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(orderDetail.Product.ProductId);
                product.Quantity = order.Type == OrderType.Sales
                    ? product.Quantity + orderDetail.Quantity
                    : product.Quantity - orderDetail.Quantity;

                _context.Products.Update(product);
            }

            // update order
            await UpdateOrder(request, order, cancellationToken);
            return new OkResult();
        }
    }
}
