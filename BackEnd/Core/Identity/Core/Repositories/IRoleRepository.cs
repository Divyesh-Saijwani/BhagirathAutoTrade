using Identity.Core.Models;

namespace Identity.Core.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> FindByNameAsync(string role);
    }
}
