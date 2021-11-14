using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Manufacture.Dtos;
using TASI.Backend.Domain.Manufacture.Entities;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Manufacture.Handlers
{
    public class ProcessManufactureCommand : IRequest<IActionResult>
    {
        public int ManufactureId { get; set; }
        public ProcessManufactureDto Body { get; set; }
    }

    public class ProcessManufactureCommandHandler : IRequestHandler<ProcessManufactureCommand, IActionResult>
    {
        private readonly ILogger<ProcessManufactureCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public ProcessManufactureCommandHandler(ILogger<ProcessManufactureCommandHandler> logger, TasiContext context,
            IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(ProcessManufactureCommand request, CancellationToken cancellationToken)
        {
            var job = await _context.Manufacture
                .Include(x => x.Product)
                .Include(x => x.StatusHistory)
                .Include(x => x.Materials)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.ManufactureId == request.ManufactureId, cancellationToken);
            if (job == null)
            {
                return new NotFoundObjectResult(new ErrorModel("Order tidak ditemukan", ErrorCodes.NotFound));
            }

            _logger.LogDebug("Transitioning order status for manufacture order ID {0}", request.ManufactureId);
            return request.Body.Code switch
            {
                ManufactureStatusCode.Queued => GetInvalidSequentialProcessResponse(),
                ManufactureStatusCode.InProcess => await TransitionToInProcess(request, job, cancellationToken),
                ManufactureStatusCode.Completed => await TransitionToCompleted(request, job, cancellationToken),
                ManufactureStatusCode.Cancelled => await TransitionToCancelled(request, job, cancellationToken),
                _ => new BadRequestObjectResult(new ErrorModel("Status tidak diketahui", ErrorCodes.ModelValidation,
                    request.Body.Code))
            };
        }

        private static IActionResult GetInvalidSequentialProcessResponse()
        {
            return new ConflictObjectResult(new ErrorModel(
                "Tidak bisa menggunakan status ini karena status sebelumnya tidak valid",
                ErrorCodes.InvalidSequentialProcess));
        }

        private async Task UpdateOrder(ProcessManufactureCommand request, ManufactureJob job,
            CancellationToken cancellationToken)
        {
            // append status
            var status = _mapper.Map<ManufactureStatus>(request.Body);
            status.Order = job;

            job.StatusHistory.Add(status);
            _context.Manufacture.Update(job);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<IActionResult> TransitionToInProcess(ProcessManufactureCommand request, ManufactureJob job,
            CancellationToken cancellationToken)
        {
            var latestStatus = job.StatusHistory.OrderBy(x => x.ModifiedDate).Last();
            if (latestStatus.Code != ManufactureStatusCode.Queued)
            {
                return GetInvalidSequentialProcessResponse();
            }

            // check stock
            if (job.Materials.Any(x => x.Product.Stock - x.Quantity < 0))
            {
                var outOfStockMaterials = job.Materials
                    .Where(x => x.Product.Stock - x.Quantity < 0)
                    .Select(x =>
                        $"{x.Product.Name} ({x.Product.Barcode}) tersedia {x.Product.Stock} dari permintaan manufaktur {x.Quantity}");
                return new ConflictObjectResult(new ErrorModel("Jumlah material untuk manufaktur kurang.",
                    ErrorCodes.NotEnoughStock, outOfStockMaterials));
            }

            // update stock
            foreach (var material in job.Materials)
            {
                material.Product.Stock -= material.Quantity;
            }

            // update order
            await UpdateOrder(request, job, cancellationToken);
            return new OkResult();
        }

        private async Task<IActionResult> TransitionToCompleted(ProcessManufactureCommand request, ManufactureJob job,
            CancellationToken cancellationToken)
        {
            var latestStatus = job.StatusHistory.OrderBy(x => x.ModifiedDate).Last();
            if (latestStatus.Code != ManufactureStatusCode.InProcess)
            {
                return GetInvalidSequentialProcessResponse();
            }

            // check final produce
            if (request.Body.FinalProduce == null)
            {
                return new ConflictObjectResult(
                    new ErrorModel("Hasil produksi harus diisi", ErrorCodes.ModelValidation));
            }

            // update stock
            job.FinalProduce = request.Body.FinalProduce.Value;
            job.Product.Stock += job.FinalProduce;

            // update order
            await UpdateOrder(request, job, cancellationToken);
            return new OkResult();
        }

        private async Task<IActionResult> TransitionToCancelled(ProcessManufactureCommand request, ManufactureJob job,
            CancellationToken cancellationToken)
        {
            var latestStatus = job.StatusHistory.OrderBy(x => x.ModifiedDate).Last();
            if (latestStatus.Code != ManufactureStatusCode.Queued && latestStatus.Code != ManufactureStatusCode.InProcess)
            {
                return GetInvalidSequentialProcessResponse();
            }

            // update order
            await UpdateOrder(request, job, cancellationToken);
            return new OkResult();
        }
    }
}
