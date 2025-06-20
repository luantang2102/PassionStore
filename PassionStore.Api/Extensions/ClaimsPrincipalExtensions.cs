using PassionStore.Core.Exceptions;
using System.Security.Claims;

namespace PassionStore.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                throw new AccessDeniedException(ErrorCode.UNAUTHORIZED_ACCESS);
            }

            try
            {
                return Guid.Parse(claim.Value);
            }
            catch (FormatException)
            {
                throw new AccessDeniedException(ErrorCode.INVALID_CLAIM);
            }
        }
    }
}
