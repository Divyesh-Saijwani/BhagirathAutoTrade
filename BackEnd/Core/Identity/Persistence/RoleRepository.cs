using Identity.Core.Repositories;
using Identity.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _context;

        public RoleRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<Role> FindByNameAsync(string role)
        {
            return await _context.Roles.Include(u => u.UsersRole)
                                       .ThenInclude(ur => ur.User)
                                       .SingleOrDefaultAsync(u => u.Name.ToLower() == role.ToLower());
        }
    }
}
