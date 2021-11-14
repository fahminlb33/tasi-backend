using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TASI.Backend.Domain.Users.Dtos;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.DataAnnotations;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;
using TASI.Backend.Infrastructure.Services;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class RegisterCommand : IRequest<IActionResult>
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string FullName { get; set; }

        [Required]
        [EnumDataType(typeof(UserRole))]
        [MustEnumValue(typeof(UserRole), new object[] {UserRole.Customer})]
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
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IActionResult>
    {
        private readonly ILogger<RegisterCommandHandler> _logger;
        private readonly IBingMapsService _bingMaps;
        private readonly DefaultTasiConfig _config;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger, IBingMapsService bingMaps, IOptions<DefaultTasiConfig> config, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _bingMaps = bingMaps;
            _config = config.Value;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
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
            user.ShippingCost = _config.FlatShippingCost * (decimal) distance;

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created user {0} with ID {1}", user.FullName, user.UserId);
            return new JsonResult(_mapper.Map<UserProfileDto>(user));
        }
    }
}
