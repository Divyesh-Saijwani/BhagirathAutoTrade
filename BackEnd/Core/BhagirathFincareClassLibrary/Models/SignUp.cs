using System;
using System.ComponentModel.DataAnnotations;

namespace BhagirathFincareClassLibrary.Models
{
    public partial class SignUp
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(20)]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(10)]
        public string MobileNumber { get; set; }


        public string SessionId { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastUpadateDate { get; set; }

        public DateTime LastPaymentDate { get; set; }

        public bool CheckBox { get; set; }

        public string SubscriptionType { get; set; }

        public bool IsAssociate { get; set; }

        public string ParentAssociateCode { get; set; }

        public string AssociateCode { get; set; }

        public string Code { get; set; }

        public string AccountType { get; set; }

        public bool AllowedEquity { get; set; }

        public bool AllowedCommodity { get; set; }

        public bool AllowedCurrency { get; set; }

        public bool AllowedHotStocks { get; set; }

        public bool AllowedSuperCallsEquity { get; set; }

        public bool AllowedSuperCallsCommodity { get; set; }

        public bool AllowedSuperCallsCurrency { get; set; }

        public string IPAddress { get; set; }

        public bool IsExpire { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsAdmin { get; set; }
        public double SubTotal { get; set; }
        public double GsTper { get; set; }
        public double Discountper { get; set; }
        public double GrandTotal { get; set; }
        public bool IsPaid { get; set; }

    }
}
