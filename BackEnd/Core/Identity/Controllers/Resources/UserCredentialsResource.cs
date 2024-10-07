using System.ComponentModel.DataAnnotations;

namespace Identity.Controllers.Resources
{
    public class UserCredentialsResource
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(32)]
        public string Password { get; set; }

        public string RoleName { get; set; } = "Common";
    }
}