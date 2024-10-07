using System;

namespace BhagirathFincareClassLibrary.Models
{
    public class Login
    {
        
        
        public string LoginUserName { get; set; }

        public string LoginPassword { get; set; }

        public string SessionId { get; set; }

        public DateTime LastUpdateDate { get; set; }
    }
}
