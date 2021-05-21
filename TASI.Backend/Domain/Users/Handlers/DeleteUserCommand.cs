using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class DeleteUserCommand : IRequest<IActionResult>
    {
        public int UserId { get; set; }
    }
    
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IActionResult>
    {
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly TasiContext _context;

        public DeleteUserCommandHandler(ILogger<CreateUserCommandHandler> logger, TasiContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Deleting user {0}", request.UserId);
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);

            return new OkResult();
        }
    }
}
