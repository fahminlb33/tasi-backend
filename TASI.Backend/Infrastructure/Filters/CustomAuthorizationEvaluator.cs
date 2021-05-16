using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TASI.Backend.Infrastructure.Filters
{
    public class CustomAuthorizationEvaluator  : IAuthorizationEvaluator
    {
        public AuthorizationResult Evaluate(AuthorizationHandlerContext context)
        {
            if (context.HasFailed == true)
            {
                return AuthorizationResult.Failed(AuthorizationFailure.ExplicitFail());
            }

            var success = context.PendingRequirements.Count() < context.Requirements.Count();

            return success 
                ? AuthorizationResult.Success()
                : AuthorizationResult.Failed(AuthorizationFailure.ExplicitFail());
        }
    }
}
