using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASI.Backend.Domain.Manufacture.Dtos;
using TASI.Backend.Domain.Orders.Dto;
using TASI.Backend.Domain.Orders.Handlers;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Manufacture.Handlers
{
    public class GetManufactureCommand : IRequest<IActionResult>
    {
        public int ManufactureId { get;set; }
    }

    public class GetManufactureCommandHandler : IRequestHandler<GetManufactureCommand, IActionResult>
    {
        private readonly ILogger<GetManufactureCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public GetManufactureCommandHandler(ILogger<GetManufactureCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(GetManufactureCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Manufacture
                .Include(x => x.StatusHistory)
                .Include(x => x.Materials)
                .FirstOrDefaultAsync(x => x.ManufactureId == request.ManufactureId, cancellationToken);
            if (order == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            _logger.LogInformation("Found manufacture order for ID {0}", order.ManufactureId);
            return new JsonResult(_mapper.Map<ManufactureJobDto>(order));
        }
    }
}
