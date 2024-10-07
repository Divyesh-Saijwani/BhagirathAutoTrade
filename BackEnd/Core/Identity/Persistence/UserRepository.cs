using Identity.Core.Models;
using Identity.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            var userRoles = user.RoleName?.Split(',').ToList();
            var roles = await _context.Roles.Where(r => userRoles!=null && userRoles.Contains(r.Name)).ToListAsync();

            foreach(var role in roles)
            {
                user.UserRoles.Add(new UserRole { RoleId = role.Id });
            }
               
            _context.Users.Add(user);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.UserRoles)
                                       .ThenInclude(ur => ur.Role)
                                       .SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var result= await _context.Users.Include(u => u.UserRoles)
                                       .ThenInclude(ur => ur.Role).ToListAsync();
            // Map RoleName property
            foreach (var user in result)
            {
                user.RoleName = user.UserRoles.FirstOrDefault()?.Role?.Name;
                user.UserRoles = null;
            }

            return result;
        }

        public async Task<User> UpdateUserDetails(User user)
        {
            var userDetails = await _context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);

            userDetails.EmailConfirmed = user.EmailConfirmed;
            userDetails.PhoneNumberConfirmed = userDetails.PhoneNumberConfirmed;

            await _context.SaveChangesAsync();

            return userDetails;
        }
    }
}