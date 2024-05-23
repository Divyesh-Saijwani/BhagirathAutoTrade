namespace BhagirathAPI.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string Exchange { get; set; }
        public string Instrument { get; set; }
        public string Symbole { get; set; }
        public string Type { get; set; }
        public DateTime WorkingDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        
        public List<StockData> Data { get; set; }
    }

    public class StockData
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public decimal StrickPrice { get; set; }
        public decimal CMP { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Average { get; set; }
        public decimal SS { get; set; }
        public string SST { get; set; }
        public decimal RS { get; set; }
        public string RST { get; set; }
        public decimal HS { get; set; }
        public decimal HR { get; set; }
        public decimal S_Bap { get; set; }
        public decimal R_Bap { get; set; }
        public decimal S3 { get; set; }
        public decimal R2 { get; set; }
        public decimal StopLoss { get; set; }
        public decimal Target { get; set; }

        public Stock Stock { get; set; }
    }
}
