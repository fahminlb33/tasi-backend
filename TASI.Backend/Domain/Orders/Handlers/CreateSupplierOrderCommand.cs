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
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Helpers;

namespace TASI.Backend.Domain.Orders.Handlers
{
    public class 
        CreateSupplierOrderCommand : IRequest<IActionResult>
    {
        [Required]
        public int SupplierId { get; set; }

        [Required]
        public List<OrderDetailDto> Products { get; set; }
    }

    public class CreateSupplierOrderCommandHandler : IRequestHandler<CreateSupplierOrderCommand, IActionResult>
    {
        private readonly ILogger<CreateSupplierOrderCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public CreateSupplierOrderCommandHandler(ILogger<CreateSupplierOrderCommandHandler> logger, TasiContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<IActionResult> Handle(CreateSupplierOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(_httpContext.GetUserIdFromClaim());
            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            var order = new Order
            {
                SupplierId = supplier.SupplierId,
                Supplier =  supplier,
                PicUserId = user.UserId,
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

            order.StatusHistory = orderStatusHistory;
            order.OrderDetails = orderDetails;
            order.TotalSales = orderDetails.Sum(x => x.TotalPrice);
            order.TotalWeight = orderDetails.Sum(x => x.TotalWeight);
            order.TotalShipping = supplier.ShippingCost * (decimal) order.TotalWeight;
            order.TotalTax = order.TotalSales * 0.10M;
            order.SubTotal = order.TotalSales + order.TotalShipping + order.TotalTax;

            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new JsonResult(order);
        }
    }
}
