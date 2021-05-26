using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Maps.Handlers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController : ControllerBase
    {
        private readonly ILogger<MapsController> _logger;
        private readonly IMediator _mediator;

        public MapsController(ILogger<MapsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("shippingCost")]
        public async Task<IActionResult> CalculateShippingCost([FromQuery, Required] double latitude, [FromQuery, Required] double longitude)
        {
            try
            {
                return await _mediator.Send(new CalculateShippingCostCommand
                {
                    Latitude = latitude,
                    Longitude = longitude
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {0}", HttpContext.Request.Path);
                return BadRequest(ErrorMessages.InternalExceptionModel);
            }
        }

        [HttpGet("reverseGeocode")]
        public async Task<IActionResult> ReverseGeocode([FromQuery, Required] string address)
        {
            try
            {
                return await _mediator.Send(new ReverseGeocodeCommand
                {
                    Address = address
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
