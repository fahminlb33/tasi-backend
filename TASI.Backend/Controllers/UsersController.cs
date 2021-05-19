using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Domain.Users.Handlers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Login(LoginCommand model)
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
        [Authorize(Roles = nameof(UserRole.SuperAdmin))]
        public async Task<IActionResult> Create(CreateUserCommand model)
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

        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.SuperAdmin))]
        public async Task<IActionResult> Delete(DeleteUserCommand model)
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

        [HttpGet]
        public async Task<IActionResult> Delete([FromQuery] GetUsersCommand model)
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

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return StatusCode((int) HttpStatusCode.Forbidden,
                        new ErrorModel(ErrorMessages.NameIdentifierNull, ErrorCodes.NameIdentifierIsEmpty));
                }

                return await _mediator.Send(new GetProfileCommand
                {
                    UserId = int.Parse(userId)
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
