using System;
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
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Helpers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Orders.Handlers
{
    public class CreateSalesOrderCommand : IRequest<IActionResult>
    {
        public int UserId { get; set; }

        [Required]
        public List<OrderCommandProductDto> Products { get; set; }
    }

    public class CreateSalesOrderCommandHandler : IRequestHandler<CreateSalesOrderCommand, IActionResult>
    {
        private readonly ILogger<CreateSalesOrderCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly DefaultTasiConfig _config;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public CreateSalesOrderCommandHandler(ILogger<CreateSalesOrderCommandHandler> logger, IMapper mapper, TasiContext context, IOptions<DefaultTasiConfig> config, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _httpContext = httpContext;
            _config = config.Value;
        }

        public async Task<IActionResult> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            User picUser;
            var loggedUser = await _context.Users.FindAsync(_httpContext.GetUserIdFromClaim());
            if (loggedUser.Role == UserRole.Customer)
            {
                picUser = loggedUser;
            }
            else
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return new NotFoundObjectResult(new ErrorModel("User tidak ditemukan", ErrorCodes.NotFound));
                }

                picUser = user;
            }
            

            var supplier = await _context.Suppliers.FindAsync(1); // const for self supplier
            var order = new Order
            {
                Supplier = supplier,
                PicUser = picUser
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

            var orderStatusHistory = new List<OrderStatus>
            {
                new()
                {
                    Code = OrderStatusCode.Requested,
                    Message = "Pesanan disimpan dan menunggu diproses.",
                    Order = order
                }
            };

            order.Type = OrderType.Sales;
            order.StatusHistory = orderStatusHistory;
            order.OrderDetails = orderDetails;
            order.TotalSales = orderDetails.Sum(x => x.TotalPrice);
            order.TotalWeight = orderDetails.Sum(x => x.TotalWeight);
            order.TotalShipping = picUser.ShippingCost * (decimal)order.TotalWeight; // shipping to user
            order.TotalTax = order.TotalSales * _config.TaxRate;
            order.SubTotal = order.TotalSales + order.TotalShipping + order.TotalTax;

            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created order with ID {0}", order.OrderId);
            return new JsonResult(_mapper.Map<OrderDto>(order));
        }
    }
}
