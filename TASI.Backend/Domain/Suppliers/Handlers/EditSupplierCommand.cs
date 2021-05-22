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
using TASI.Backend.Domain.Suppliers.Dtos;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

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

        public EditSupplierCommandHandler(ILogger<EditSupplierCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(EditSupplierCommand request, CancellationToken cancellationToken)
        {
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

            _logger.LogInformation("Updated supplier with ID {0}", supplier.SupplierId);
            return new JsonResult(supplier);
        }
    }
}
