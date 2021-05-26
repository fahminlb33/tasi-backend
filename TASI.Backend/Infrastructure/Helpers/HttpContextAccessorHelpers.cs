using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TASI.Backend.Infrastructure.Helpers
{
    public static class HttpContextAccessorHelpers
    {
        public static int GetUserIdFromClaim(this IHttpContextAccessor httpContextAccessor)
        {
            var idFromClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idFromClaim, out var id))
            {
                return id;
            }

            throw new Exception("Tidak terdapat user ID pada claim.");
        }
    }
}
