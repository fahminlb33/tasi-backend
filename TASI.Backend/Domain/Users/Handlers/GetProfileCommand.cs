using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Users.Dtos;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Helpers;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class GetProfileCommand : IRequest<IActionResult>
    {
        public int? UserId { get; set; }
    }
    
    public class GetProfileCommandHandler : IRequestHandler<GetProfileCommand, IActionResult>
    {
        private readonly ILogger<GetProfileCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public GetProfileCommandHandler(TasiContext context, IMapper mapper, ILogger<GetProfileCommandHandler> logger, IHttpContextAccessor httpContext)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _httpContext = httpContext;
        }

        public async Task<IActionResult> Handle(GetProfileCommand request, CancellationToken cancellationToken)
        {
            request.UserId ??= _httpContext.GetUserIdFromClaim();
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            _logger.LogInformation("Found user profile for ID {0}", user.UserId);
            return new JsonResult(_mapper.Map<UserProfileDto>(user));
        }
    }
}
