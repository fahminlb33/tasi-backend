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
using TASI.Backend.Domain.Users.Dto;
using TASI.Backend.Domain.Users.Entities;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class GetUsersCommand : IRequest<IActionResult>
    {
        public int Page { get; set; }
        public int Limit { get; set; } = 10;
        public string? Query { get; set; }
        public UserRole? Role { get;set; }
    }
    
    public class GetUsersCommandHandler : IRequestHandler<GetUsersCommand, IActionResult>
    {
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;

        public GetUsersCommandHandler(ILogger<CreateUserCommandHandler> logger, TasiContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(GetUsersCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Finding users {0} and role {1}", request.Query, request.Role);

            var skipCount = request.Page * request.Limit;
            var query = _context.Users.AsQueryable();
            if (request.Query != null)
            {
                query = query.Where(x => EF.Functions.Like(x.FullName, $"%{request.Query}%"));
            }

            if (request.Role != null)
            {
                query = query.Where(x => x.Role == request.Role);
            }

            var result = await query.Skip(skipCount).Take(request.Limit).ToListAsync(cancellationToken);
            var totalRecords = await query.CountAsync(cancellationToken);
            var resultDto = _mapper.Map<IList<UserProfileDto>>(result);

            return new JsonResult(new Pagination<UserProfileDto>(request.Page, result.Count, totalRecords, resultDto));
        }
    }
}