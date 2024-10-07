using Identity.Core.Models;
using Identity.Services.Communication;

namespace Identity.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<CreateUserResponse> CreateUserAsync(User user);
        Task<User> FindByEmailAsync(string email);
        Task<string> GetEmailVerificationToken(string email);
        Task<EmailVerificationResponse> VerifyEmail(string email, string token);
    }
}