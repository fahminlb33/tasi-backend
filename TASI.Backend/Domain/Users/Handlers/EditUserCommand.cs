#nullable enable

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
    public class EditUserCommand : IRequest<IActionResult>
    {
        public int UserId { get; set; }
        
        public EditUserCommandBody? Body { get; set; }
    }

    public class EditUserCommandBody
    {
        public string? FullName { get; set; }
        public UserRole? Role { get; set; }
        public string? Username { get; set; }
    }
    
    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, IActionResult>
    {
        private readonly ILogger<EditUserCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public EditUserCommandHandler(ILogger<EditUserCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating user {0}", request.UserId);

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            if (request.Body?.FullName != null)
            {
                if (await _context.Users.AnyAsync(x => x.FullName.ToLower() == request.Body.FullName.ToLower(), cancellationToken))
                {
                    return new ConflictObjectResult(new ErrorModel("Nama sudah digunakan oleh akun lain",
                        ErrorCodes.DataDuplicated));
                }
            }
            
            if (request.Body?.Username != null)
            {
                if (await _context.Users.AnyAsync(x => x.FullName.ToLower() == request.Body.Username.ToLower(), cancellationToken))
                {
                    return new ConflictObjectResult(new ErrorModel("Username sudah digunakan oleh akun lain",
                        ErrorCodes.DataDuplicated));
                }
            }

            var updatedEntity = _mapper.Map(request.Body, user);
            _context.Users.Update(updatedEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return new JsonResult(_mapper.Map<UserProfileDto>(await _context.Users.FindAsync(user.UserId)));
        }
    }
}
