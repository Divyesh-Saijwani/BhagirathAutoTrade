namespace BhagirathAutoTrade.Server.Models
{
    public class EquityDataModel
    {
        public int Id { get; set; }
        public string Symbole { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal StrikePrice { get; set; }
        public string OptionType { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Cmp { get; set; }
        public decimal Close { get; set; }
        public int FileType { get; set; }
        public DateTime UploadDateTime { get; set; }
        public int FileNo { get; set; }
        public decimal Average { get; set; }
        public long TotalTradeValue { get; set; }
    }
}
