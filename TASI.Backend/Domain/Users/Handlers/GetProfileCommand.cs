using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Users.Dto;
using TASI.Backend.Infrastructure.Database;
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
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public GetProfileCommandHandler(TasiContext context, IMapper mapper, ILogger<GetProfileCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(GetProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting user profile for ID {0}", request.UserId);
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            return new JsonResult(_mapper.Map<UserProfileDto>(user));
        }
    }
}
