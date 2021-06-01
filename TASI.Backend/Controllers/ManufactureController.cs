using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Manufacture.Dtos;
using TASI.Backend.Domain.Manufacture.Handlers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ManufactureController : ControllerBase
    {
        private readonly ILogger<ManufactureController> _logger;
        private readonly IMediator _mediator;

        public ManufactureController(ILogger<ManufactureController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateManufactureCommand model)
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

        [HttpPost("{manufactureId}/process")]
        public async Task<IActionResult> Process([FromRoute, Required] int manufactureId, [FromBody] ProcessManufactureDto model)
        {
            try
            {
                return await _mediator.Send(new ProcessManufactureCommand
                {
                    ManufactureId = manufactureId,
                    Body = model
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
