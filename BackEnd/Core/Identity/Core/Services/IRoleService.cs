using Identity.Core.Models;

namespace Identity.Core.Services
{
    public interface IRoleService
    {
        Task<Role> FindByNameAsync(string role);
    }
}
