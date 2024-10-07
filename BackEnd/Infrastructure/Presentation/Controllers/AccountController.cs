using Contracts;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Identity.Security;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public AccountController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize]
        [HttpGet("GetUserDetails")]
        public async Task<UserResponseDTO> GetUserDetails()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claims = new Claims(HttpContext);
                var roles = claims.Role?.Split(',').ToList();
                var user = await _serviceManager.AccountService.GetIdentityUserData(claims.Email);
                user.Email = claims.Email;
                user.FullName = claims.Email;
                user.PrimaryRole=roles.FirstOrDefault();
                user.Roles = roles;
                return user;
            }

            return null;
        }
    }
}
