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
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.DataAnnotations;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;
using TASI.Backend.Infrastructure.Services;

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

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; }

        [Required]
        public long Latitude { get; set; }

        [Required]
        public long Longitude { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IActionResult>
    {
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;
        private readonly IBingMapsService _bingMaps;
        private readonly DefaultTasiConfig _config;

        public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger, TasiContext context, IMapper mapper, IBingMapsService bingMaps, DefaultTasiConfig config)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _bingMaps = bingMaps;
            _config = config;
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
            var distance = await _bingMaps.CalculateDistance(user.Latitude, user.Longitude, _config.CompanyLatitude,
                _config.CompanyLongitude, cancellationToken);
            user.ShippingCost = _config.FlatShippingCost * (decimal)distance;

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created user {0} with ID {1}", user.FullName, user.UserId);
            return new JsonResult(_mapper.Map<UserProfileDto>(user));
        }
    }
}
