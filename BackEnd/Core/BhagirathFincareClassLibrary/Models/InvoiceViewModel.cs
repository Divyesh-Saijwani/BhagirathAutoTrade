namespace BhagirathFincareClassLibrary.Models
{
    public partial class InvoiceViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string SubscriptionType { get; set; }
        public bool AllowedEquity { get; set; }
        public bool AllowedCommodity { get; set; }
        public bool AllowedCurrency { get; set; }
        public double SubTotal { get; set; }
        public double Gstper { get; set; }
        public double Discountper { get; set; }
        public double GrandTotal { get; set; }
        public bool IsPaid { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.DateTime LastPaymentDate { get; set; }
        public System.DateTime InvoiceValidity { get; set; }
        public int PaymentType { get; set; }
        public string InvoiceNo { get; set; }
        public double CustomizedPrice { get; set; }

    }
}
