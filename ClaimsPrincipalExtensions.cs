using System.Security.Claims;

namespace Campus360{
    public static class ClaimsPrincipalExtensions{
        public static string GetUserId(this ClaimsPrincipal user){
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    } 
}