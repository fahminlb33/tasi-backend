using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Helpers;

namespace TASI.Backend.Domain.Orders.Handlers
{
    public class CreateSupplierOrderCommand : IRequest<IActionResult>
    {
        [Required]
        public int SupplierId { get; set; }

        [Required]
        public List<CreateSupplierCommandProductsDto> Products { get; set; }
    }

    public class CreateSupplierCommandProductsDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

    public class CreateSupplierOrderCommandHandler : IRequestHandler<CreateSupplierOrderCommand, IActionResult>
    {
        private readonly ILogger<CreateSupplierOrderCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly TasiContext _context;
        private readonly DefaultTasiConfig _config;

        public CreateSupplierOrderCommandHandler(ILogger<CreateSupplierOrderCommandHandler> logger, TasiContext context, IHttpContextAccessor httpContext, IOptions<DefaultTasiConfig> config, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _httpContext = httpContext;
            _mapper = mapper;
            _config = config.Value;
        }

        public async Task<IActionResult> Handle(CreateSupplierOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(_httpContext.GetUserIdFromClaim());
            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            var order = new Order
            {
                Supplier =  supplier,
                PicUser = user
            };

            var orderDetails = await Task.WhenAll(request.Products.Select(async x =>
            {
                var product = await _context.Products.FindAsync(x.ProductId);
                return new OrderDetail
                {
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * x.Quantity,
                    TotalWeight = product.Weight * x.Quantity,
                    Quantity = x.Quantity,
                    Unit = product.Unit,
                    Product = product,
                    Order = order
                };
            }));

            var orderStatusHistory = new List<OrderStatusHistory>
            {
                new()
                {
                    Code = OrderStatus.Requested,
                    Message = "Pesanan disimpan dan menunggu diproses.",
                    Order = order
                }
            };

            order.Type = OrderType.Supplier;
            order.StatusHistory = orderStatusHistory;
            order.OrderDetails = orderDetails;
            order.TotalSales = orderDetails.Sum(x => x.TotalPrice);
            order.TotalWeight = orderDetails.Sum(x => x.TotalWeight);
            order.TotalShipping = supplier.ShippingCost * (decimal) order.TotalWeight;
            order.TotalTax = order.TotalSales * _config.TaxRate;
            order.SubTotal = order.TotalSales + order.TotalShipping + order.TotalTax;

            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created order with ID {0}", order.OrderId);
            return new JsonResult(_mapper.Map<OrderDto>(order));
        }
    }
}
