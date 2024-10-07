using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Identity.Security
{
    public class Claims
    {
        private readonly ClaimsIdentity Identity;

        public Claims(HttpContext context)
        {
            Identity = (ClaimsIdentity?)context.User.Identity;
        }

        /// <summary>
        /// Get claim from request object
        /// </summary>
        /// <param name="claimName"></param>
        /// <returns></returns>

        public string GetClaim(string claimName)
        {
            
            return Identity.Claims.FirstOrDefault(x=>x.Type == claimName).Value;
        }

        /// <summary>
        /// To return user id from claims
        /// </summary>
        public  string UserId => GetClaim(ClaimTypes.NameIdentifier);

        /// <summary>
        /// To return user full name from claims
        /// </summary>
        public  string UserName => GetClaim(ClaimTypes.Email);

        /// <summary>
        /// To return clientId from claims
        /// </summary>
        public  string Email => GetClaim(ClaimTypes.Email);

        /// <summary>
        /// To return roles from claims
        /// </summary>
        public  string Role => GetClaim(ClaimTypes.Role);
    }
}
