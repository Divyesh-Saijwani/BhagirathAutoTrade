using AutoMapper;
using Identity.Controllers.Resources;
using Identity.Core.Services;
using Identity.Core.Models;
using Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UsersController(IUserService userService, IRoleService roleService, IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
        }

        [HttpGet("List")]
        public async Task<IEnumerable<User>> GetAsync() {
            var response = await _userService.GetUsersAsync();
            return response;
        }

        [HttpGet("GetByEmailId")]
        public async Task<IActionResult> GetByEmailAsync(string email)
        {
            var response = await _userService.FindByEmailAsync(email);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserCredentialsResource userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<UserCredentialsResource, User>(userCredentials);
            
            var response = await _userService.CreateUserAsync(user);
            if(!response.Success)
            {
                return BadRequest(response.Message);
            }

            var userResource = _mapper.Map<User, UserResource>(response.User);
            return Ok(userResource);
        }

        [HttpGet("verifyemail")]
        public async Task<IActionResult> VerifyEmail(string email,string token)
        {
            var response=await _userService.VerifyEmail(email, token);
            return Ok(response);
        }

        [HttpGet("generate-security-token")]
        public async Task<IActionResult> GenerateSecurityToken(string email)
        {
            var response = await _userService.GetEmailVerificationToken(email);
            return Ok(new {token=response});
        }
    }
}