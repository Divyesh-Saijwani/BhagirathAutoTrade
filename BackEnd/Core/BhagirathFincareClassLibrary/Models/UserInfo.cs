using System;

namespace BhagirathFincareClassLibrary.Models
{
    public partial class UserInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string SessionId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public System.DateTime LastUpadateDate { get; set; }
        public bool IsExpire { get; set; }
        public System.DateTime LastPaymentDate { get; set; }
        public int PaymentType { get; set; }
        public string Email { get; set; }
        public string SubscriptionType { get; set; }
        public bool IsAssociate { get; set; }
        public string ParentAssociateCode { get; set; }
        public string AssociateCode { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public bool AllowedEquity { get; set; }
        public bool AllowedCommodity { get; set; }
        public bool AllowedCurrency { get; set; }
        public bool? AllowedHotStocks { get; set; }
        public bool? AllowedSuperCallsEquity { get; set; }
        public bool? AllowedSuperCallsCommodity { get; set; }
        public bool? AllowedSuperCallsCurrency { get; set; }
        public string IPAddress { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
        public int NoOfDays { get; set; }
    }
}
