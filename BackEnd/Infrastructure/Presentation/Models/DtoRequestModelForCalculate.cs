namespace BhagirathAutoTrade.Model
{
    public class DtoRequestModelForCalculate
    {
        public string WorkingDate { get; set; }
        public string ExpiryDate { get; set; }
        public string Exchange { get; set; }
        public string Instrument { get; set; }
        public string Symbole { get; set; }
        public string Type { get; set; }
        public string StrickPrice { get; set; }
        public decimal Close { get; set; }
        public decimal Average { get; set; }
        public decimal CMP { get; set; }
        public decimal Open { get; set; }
        public decimal IDH { get; set; }
        public decimal IDL { get; set; }
        public string OptionType { get; set; }
        public string EMA13 { get; set; }
        public string EMA34 { get; set; }
    }
}
