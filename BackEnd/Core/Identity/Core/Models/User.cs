using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Core.Models
{
    public class User : IdentityUser<Guid>
    { 
        [NotMapped]
        public virtual string? RoleName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
    }
}