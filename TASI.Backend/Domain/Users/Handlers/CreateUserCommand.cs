using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Users.Dtos;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Infrastructure.DataAnnotations;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class CreateUserCommand : IRequest<IActionResult>
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string FullName { get; set; }

        [Required]
        [EnumDataType(typeof(UserRole))]
        [NotEnumValue(typeof(UserRole), new object[] { UserRole.SuperAdmin })]
        public UserRole Role { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
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

            _logger.LogInformation("Created user {0} with ID {1}", user.FullName, user.UserId);
            return new JsonResult(_mapper.Map<UserProfileDto>(user));
        }
    }
}
