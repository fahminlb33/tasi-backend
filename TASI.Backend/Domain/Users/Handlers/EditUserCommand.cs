#nullable enable

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Users.Dtos;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;
using TASI.Backend.Infrastructure.Services;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class EditUserCommand : IRequest<IActionResult>
    {
        public int UserId { get; set; }
        
        public EditUserDto? Body { get; set; }
    }

    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, IActionResult>
    {
        private readonly ILogger<EditUserCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;
        private readonly IBingMapsService _bingMaps;
        private readonly DefaultTasiConfig _config;

        public EditUserCommandHandler(ILogger<EditUserCommandHandler> logger, TasiContext context, IMapper mapper, IBingMapsService bingMaps, DefaultTasiConfig config)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _bingMaps = bingMaps;
            _config = config;
        }

        public async Task<IActionResult> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            // make sure no duplicate fullname
            if (request.Body?.FullName != null)
            {
                if (await _context.Users.AnyAsync(x => x.FullName.ToLower() == request.Body.FullName.ToLower(), cancellationToken))
                {
                    return new ConflictObjectResult(new ErrorModel("Nama sudah digunakan oleh akun lain",
                        ErrorCodes.DataDuplicated));
                }
            }
            
            // make sure no duplicate username
            if (request.Body?.Username != null)
            {
                if (await _context.Users.AnyAsync(x => x.FullName.ToLower() == request.Body.Username.ToLower(), cancellationToken))
                {
                    return new ConflictObjectResult(new ErrorModel("Username sudah digunakan oleh akun lain",
                        ErrorCodes.DataDuplicated));
                }
            }

            // calculate shipping cost
            if (request.Body?.Latitude != null &&
                request.Body?.Longitude != null &&
                _bingMaps.IsPointDifferent(user.Latitude,
                    user.Longitude,
                    request.Body.Latitude.Value,
                    request.Body.Longitude.Value))
            {
                var distance = await _bingMaps.CalculateDistance(request.Body.Latitude.Value,
                    request.Body.Longitude.Value, _config.CompanyLatitude, _config.CompanyLongitude, cancellationToken);
                user.ShippingCost = _config.FlatShippingCost * (decimal)distance;
            }

            // project and save the entity
            var updatedEntity = _mapper.Map(request.Body, user);
            _context.Users.Update(updatedEntity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated user with ID {0}", user.UserId);
            return new JsonResult(_mapper.Map<UserProfileDto>(user));
        }
    }
}
