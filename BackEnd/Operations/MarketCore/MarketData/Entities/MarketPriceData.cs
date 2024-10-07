namespace MarketData.Entities
{
    public class MarketPriceResponse
    {
        public List<MarketPriceData> Success { get; set; }
        public int Status { get; set; }
        public object Error { get; set; }
    }

    public class MarketPriceData
    {
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime UploadDate { get; set; }
        public string StockCode { get; set; }
        public string ExchangeCode { get; set; }
        public string OptionType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public object Right { get; set; }
        public object StrikePrice { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
        public int FileType { get; set; }
        public object OpenInterest { get; set; }
        public int Count { get; set; }
        public string TotalTradeValue { get; set; }
    }
}
