using Identity.Core.Models;

namespace Identity.Core.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task AddAsync(User user);
        Task<User> FindByEmailAsync(string email);
        Task<User> UpdateUserDetails(User user);
    }
}