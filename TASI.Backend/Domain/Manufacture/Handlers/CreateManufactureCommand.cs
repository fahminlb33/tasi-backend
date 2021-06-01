using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Manufacture.Dtos;
using TASI.Backend.Domain.Manufacture.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Manufacture.Handlers
{
    public class CreateManufactureCommand : IRequest<IActionResult>
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ExpectedProduce { get; set; }

        [Required]
        public DateTime ExpectedCompletion { get; set; }

        [Required]
        public List<CreateManufactureMaterialDto> Materials { get; set; }
    }

    public class CreateManufactureCommandHandler : IRequestHandler<CreateManufactureCommand, IActionResult>
    {
        private readonly ILogger<CreateManufactureCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public CreateManufactureCommandHandler(ILogger<CreateManufactureCommandHandler> logger, TasiContext context,
            IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(CreateManufactureCommand request, CancellationToken cancellationToken)
        {
            // find the manufactured product
            var product = await _context.Products.FindAsync(new object[] {request.ProductId}, cancellationToken);

            // check if we can manufacture the product
            if (!product.CanManufacture)
            {
                return new ConflictObjectResult(new ErrorModel("Barang tidak dapat diproduksi",
                    ErrorCodes.ModelValidation));
            }

            // create new manufacture job
            var job = new ManufactureJob
            {
                Product = product,
                ExpectedProduce = request.ExpectedProduce,
                ExpectedCompletion = request.ExpectedCompletion
            };

            // get all the required materials
            job.Materials = await Task.WhenAll(request.Materials.Select(async x =>
            {
                var material = await _context.Products.FindAsync(new object[] {x.ProductId}, cancellationToken);
                return new ManufactureMaterial
                {
                    Order = job,
                    Product = material,
                    Quantity = x.Quantity
                };
            }));

            // create history
            job.StatusHistory = new List<ManufactureStatus>
            {
                new()
                {
                    Code = ManufactureStatusCode.Queued,
                    Message = "Manufaktur diantrekan.",
                    Order = job
                }
            };

            // save changes
            await _context.Manufacture.AddAsync(job, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created manufacture job with ID {0}", job);
            return new JsonResult(_mapper.Map<SimpleManufactureJobDto>(job));
        }
    }
}
