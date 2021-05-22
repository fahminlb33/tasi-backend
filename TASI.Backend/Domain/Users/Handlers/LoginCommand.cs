using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TASI.Backend.Domain.Users.Dto;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain.Users.Handlers
{
    public class LoginCommand : IRequest<IActionResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class LoginCommandHandler : IRequestHandler<LoginCommand, IActionResult>
    {
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly TasiContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public LoginCommandHandler(TasiContext context, IConfiguration config, IMapper mapper, ILogger<LoginCommandHandler> logger)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {0} login attempt", request.Username);

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == request.Username, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return new UnauthorizedObjectResult(new ErrorModel(ErrorMessages.Unauthorized, ErrorCodes.Unauthorized));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:EncryptionKey"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                })
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("User {0} with ID {1} logged in", user.Username, user.UserId);
            return new JsonResult(new LoginResponseDto
            {
                Profile = _mapper.Map<UserProfileDto>(user),
                AccessToken = tokenHandler.WriteToken(token)
            });
        }
    }
}
