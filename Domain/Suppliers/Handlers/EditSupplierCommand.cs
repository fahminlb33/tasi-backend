#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TASI.Backend.Domain.Suppliers.Dtos;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;
using TASI.Backend.Infrastructure.Services;

namespace TASI.Backend.Domain.Suppliers.Handlers
{
    public class EditSupplierCommand : IRequest<IActionResult>
    {
        public int SupplierId { get; set; }
        public EditSupplierDto? Body { get; set; }
    }

    public class EditSupplierCommandHandler : IRequestHandler<EditSupplierCommand, IActionResult>
    {
        private readonly ILogger<EditSupplierCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;
        private readonly IBingMapsService _bingMaps;
        private readonly DefaultTasiConfig _config;

        public EditSupplierCommandHandler(ILogger<EditSupplierCommandHandler> logger, TasiContext context,
            IMapper mapper, IBingMapsService bingMaps, IOptions<DefaultTasiConfig> config)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _bingMaps = bingMaps;
            _config = config.Value;
        }

        public async Task<IActionResult> Handle(EditSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers.FindAsync(new object[] {request.SupplierId}, cancellationToken);
            if (supplier == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            // make sure no duplicate name
            if (request.Body?.Name != null && !string.Equals(supplier.Name, request.Body.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (await _context.Suppliers.AnyAsync(x => x.Name.ToLower() == request.Body.Name.ToLower(),
                    cancellationToken))
                {
                    return new ConflictObjectResult(new ErrorModel("Nama sudah digunakan pada supplier sebelumnya.",
                        ErrorCodes.DataDuplicated));
                }
            }

            // calculate shipping cost
            if (request.Body?.Latitude != null &&
                request.Body?.Longitude != null &&
                _bingMaps.IsPointDifferent(supplier.Latitude,
                    supplier.Longitude,
                    request.Body.Latitude.Value,
                    request.Body.Longitude.Value))
            {
                var distance = await _bingMaps.CalculateDistance(request.Body.Latitude.Value,
                    request.Body.Longitude.Value, _config.CompanyLatitude, _config.CompanyLongitude, cancellationToken);
                supplier.ShippingCost = _config.FlatShippingCost * (decimal) distance;
            }

            var updatedEntity = _mapper.Map(request.Body, supplier);
            _context.Suppliers.Update(updatedEntity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated supplier with ID {0}", supplier.SupplierId);
            return new JsonResult(supplier);
        }
    }
}
