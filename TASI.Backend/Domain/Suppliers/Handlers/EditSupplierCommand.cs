#nullable enable

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
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Suppliers.Handlers
{
    public class EditSupplierCommand : IRequest<IActionResult>
    {
        public int SupplierId { get; set; }
        public EditSupplierCommandBody? Body { get; set; }
    }

    public class EditSupplierCommandBody
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
    }

    public class EditSupplierCommandHandler : IRequestHandler<EditSupplierCommand, IActionResult>
    {    
        private readonly ILogger<EditSupplierCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public EditSupplierCommandHandler(ILogger<EditSupplierCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(EditSupplierCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating supplier {0}", request.SupplierId);

            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            if (supplier == null)
            {
                return new NotFoundObjectResult(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound));
            }

            if (request.Body?.Name != null)
            {
                if (await _context.Suppliers.AnyAsync(x => x.Name.ToLower() == request.Body.Name.ToLower()))
                {
                    return new ConflictObjectResult(new ErrorModel("Nama sudah digunakan pada supplier sebelumnya.",
                        ErrorCodes.DataDuplicated));
                }
            }

            var updatedEntity = _mapper.Map(request.Body, supplier);
            _context.Suppliers.Update(updatedEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return new JsonResult(await _context.Suppliers.FindAsync(supplier.SupplierId));
        }
    }
}
