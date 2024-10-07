using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace Identity.Core.Models
{
    public class Role:IdentityRole<Guid>
    {
        public ICollection<UserRole> UsersRole { get; set; } = new Collection<UserRole>();
    }
}