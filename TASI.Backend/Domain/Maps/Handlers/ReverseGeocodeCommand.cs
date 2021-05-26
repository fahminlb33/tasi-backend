using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Infrastructure.Resources;
using TASI.Backend.Infrastructure.Services;

namespace TASI.Backend.Domain.Maps.Handlers
{
    public class ReverseGeocodeCommand : IRequest<IActionResult>
    {
        [Required]
        public string Address { get; set; }
    }

    public class ReverseGeocodeCommandHandler : IRequestHandler<ReverseGeocodeCommand, IActionResult>
    {
        private readonly ILogger<ReverseGeocodeCommandHandler> _logger;
        private readonly IBingMapsService _bing;

        public ReverseGeocodeCommandHandler(ILogger<ReverseGeocodeCommandHandler> logger, IBingMapsService bing)
        {
            _logger = logger;
            _bing = bing;
        }

        public async Task<IActionResult> Handle(ReverseGeocodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reverse geocoding location {0}...", request.Address);
            var result = await _bing.ReverseGeocode(request.Address, cancellationToken);

            if (!result.Success)
            {
                return new NotFoundObjectResult(new ErrorModel("Lokasi tidak dapat ditemukan", ErrorCodes.NotFound, request.Address));
            }

            _logger.LogInformation("Geocoded location of {0} is {1},{2} at {3}", request.Address, result.Latitude, result.Longitude, result.GeocodedAddress);
            return new JsonResult(result);
        }
    }
}
