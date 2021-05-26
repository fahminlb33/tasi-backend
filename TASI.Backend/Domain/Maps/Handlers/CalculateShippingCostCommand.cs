using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TASI.Backend.Domain.Maps.Dtos;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Services;

namespace TASI.Backend.Domain.Maps.Handlers
{
    public class CalculateShippingCostCommand: IRequest<IActionResult>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CalculateShippingCostCommandHandler : IRequestHandler<CalculateShippingCostCommand, IActionResult>
    {
        private readonly ILogger<CalculateShippingCostCommandHandler> _logger;
        private readonly IBingMapsService _bing;
        private readonly DefaultTasiConfig _config;

        public CalculateShippingCostCommandHandler(ILogger<CalculateShippingCostCommandHandler> logger, IBingMapsService bing, IOptions<DefaultTasiConfig> config)
        {
            _logger = logger;
            _bing = bing;
            _config = config.Value;
        }

        public async Task<IActionResult> Handle(CalculateShippingCostCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calculating distance matrix for {0},{1}", request.Latitude, request.Longitude);
            var distance = await _bing.CalculateDistance(request.Latitude, request.Longitude, _config.CompanyLatitude,
                _config.CompanyLongitude, cancellationToken);
            var totalPrice = _config.FlatShippingCost * (decimal)distance;

            _logger.LogInformation("Distance matrix for {0},{1} to company origin is {2} Km", request.Latitude, request.Longitude, distance);
            return new JsonResult(new ShippingCostDto(request.Latitude, request.Longitude, distance, totalPrice));
        }
    }
}
