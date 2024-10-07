namespace BhagirathFincareClassLibrary.Models
{
    public partial class LocalPasswordModel
    {
       
        public int UserId { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }


    }
}
