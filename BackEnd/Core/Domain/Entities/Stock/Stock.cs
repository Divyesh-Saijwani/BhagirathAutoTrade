using AlgoBhagirath.Common.Enums;

namespace Domain.Entities.Stock
{
    public class Stock
    {
        public Guid StockId { get; set; }
        public string StockName { get; set; }
        public string StockCode { get; set; }
        public ExchangeType Exchange { get; set; }
    }
}
