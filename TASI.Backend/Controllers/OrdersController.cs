using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Domain.Orders.Handlers;
using TASI.Backend.Domain.Users.Dtos;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Domain.Users.Handlers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IMediator _mediator;

        public OrdersController(ILogger<OrdersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("supplier")]
        public async Task<IActionResult> CreateSupplierOrder([FromBody] CreateSupplierOrderCommand model)
        {
            try
            {
                return await _mediator.Send(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return BadRequest(ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpPost("sales")]
        public async Task<IActionResult> CreateSalesOrder([FromBody] CreateSalesOrderCommand model)
        {
            try
            {
                return await _mediator.Send(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return BadRequest(ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetOrdersCommand model)
        {
            try
            {
                return await _mediator.Send(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return BadRequest(ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> Get([FromRoute] int orderId)
        {
            try
            {
                return await _mediator.Send(new GetOrderCommand
                {
                    OrderId = orderId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return BadRequest(ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpPost("{orderId}/process")]
        public async Task<IActionResult> Process([FromRoute, Required] int orderId, [FromBody] ProcessOrderDto body)
        {
            try
            {
                return await _mediator.Send(new ProcessOrderCommand
                {
                    OrderId = orderId,
                    Body = body
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return BadRequest(ErrorMessages.InternalExceptionModel);
            }
        }
    }
}
