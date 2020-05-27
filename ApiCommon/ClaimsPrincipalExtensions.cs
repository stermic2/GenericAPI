using System.Linq;
using System.Security.Claims;

namespace WebstoreData.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.Claims.FirstOrDefault(x => x.Type == "sub") ??
                        principal.Claims.FirstOrDefault(x =>
                            x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (claim == null) return null;

            return int.Parse(claim.Value);
        }
    }
}