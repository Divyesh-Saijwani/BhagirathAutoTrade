namespace BhagirathFincareClassLibrary.Models
{
    public  class ResetPasswordConfirmModel
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
