using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TASI.Backend.Domain;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class AuthRegistration
    {
        public static void AddCustomAuth(this IServiceCollection services, IConfiguration config)
        {
            var jwtConfig = config.GetSection(nameof(JwtConfig)).Get<JwtConfig>();
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.EncryptionKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            await context.Response.WriteAsJsonAsync(new ErrorModel(ErrorMessages.Unauthorized,
                                ErrorCodes.Unauthorized));
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await context.Response.WriteAsJsonAsync(new ErrorModel(ErrorMessages.Forbidden,
                                ErrorCodes.Forbidden));
                        }
                    };
                });
            services.AddAuthorization();
        }

        public static void UseCustomAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
