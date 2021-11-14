using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Maps.Dtos;
using TASI.Backend.Infrastructure.Resources;
using TASI.Backend.Infrastructure.Services;

namespace TASI.Backend.Domain.Maps.Handlers
{
    public class LookupAddressCommand : IRequest<IActionResult>
    {
        [Required]
        public string Address { get; set; }
    }

    public class LookupAddressCommandHandler : IRequestHandler<LookupAddressCommand, IActionResult>
    {
        private readonly ILogger<LookupAddressCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IBingMapsService _bing;

        public LookupAddressCommandHandler(ILogger<LookupAddressCommandHandler> logger, IBingMapsService bing,
            IMapper mapper)
        {
            _logger = logger;
            _bing = bing;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(LookupAddressCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reverse geocoding location {0}...", request.Address);
            var result = await _bing.ReverseGeocode(request.Address, cancellationToken);

            if (!result.Success)
            {
                return new NotFoundObjectResult(new ErrorModel("Lokasi tidak dapat ditemukan", ErrorCodes.NotFound,
                    request.Address));
            }

            _logger.LogInformation("Geocoded location of {0} is {1},{2} at {3}", request.Address, result.Latitude,
                result.Longitude, result.GeocodedAddress);
            return new JsonResult(_mapper.Map<LookupAddressDto>(result));
        }
    }
}
