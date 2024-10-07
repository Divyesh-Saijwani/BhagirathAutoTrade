using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Calculation
{
    public class CalculationDataDto
    {
        public Guid AlgoCalculationId { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public string Type { get; set; }
        public string Instrument { get; set; }
        public string OptionType { get; set; }
        public decimal StrikePrice { get; set; }
        public DateTime Expiry { get; set; }
        public string Trend { get; set; }
        public decimal LowPoint { get; set; }
        public decimal AveragePoint { get; set; }
        public decimal MaxPoint { get; set; }
        public string Direction { get; set; }
        public DateTime EntryTime { get; set; }
        public decimal StopLoss { get; set; }
        public decimal Target { get; set; }
        public DateTime CalculationDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
