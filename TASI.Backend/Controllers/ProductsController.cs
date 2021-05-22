using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Products.Dtos;
using TASI.Backend.Domain.Products.Handlers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediator _mediator;

        public ProductsController(ILogger<ProductsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> Get([FromRoute, Required] int productId)
        {
            try
            {
                return await _mediator.Send(new GetProductCommand
                {
                    ProductId = productId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return StatusCode((int)HttpStatusCode.InternalServerError, ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsCommand model)
        {
            try
            {
                return await _mediator.Send(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return StatusCode((int)HttpStatusCode.InternalServerError, ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody, Required] CreateProductCommand model)
        {
            try
            {
                return await _mediator.Send(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return StatusCode((int)HttpStatusCode.InternalServerError, ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Edit([FromRoute, Required] int productId, [FromBody] EditProductDto body)
        {
            try
            {
                return await _mediator.Send(new EditProductCommand
                {
                    ProductId = productId,
                    Body = body
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return StatusCode((int)HttpStatusCode.InternalServerError, ErrorMessages.InternalExceptionModel);
            }
        }
    }
}
