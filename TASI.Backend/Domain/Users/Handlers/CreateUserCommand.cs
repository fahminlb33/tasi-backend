using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Users.Dto;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class CreateUserCommand : IRequest<IActionResult>
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IActionResult>
    {
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Creating new user {0}", request.FullName);
            if (await _context.Users.AnyAsync(x =>
                x.Username.ToLower() == request.Username.ToLower() ||
                x.FullName.ToLower() == request.FullName.ToLower(), cancellationToken))
            {
                return new ConflictObjectResult(new ErrorModel("Username atau nama lengkap sudah ada sebelumnya",
                    ErrorCodes.ModelValidation));
            }

            var user = _mapper.Map<User>(request);
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new JsonResult(_mapper.Map<UserProfileDto>(await _context.Users.FindAsync(user.UserId)));
        }
    }
}
