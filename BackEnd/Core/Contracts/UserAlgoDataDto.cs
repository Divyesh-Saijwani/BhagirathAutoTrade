using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class UserAlgoDataDto
    {
        public string? WorkingDate { get; set; } // Maps to `WorkingDate` (nullable string)
        public string Symbol { get; set; } // Maps to `symbol`
        public string Exchange { get; set; }  // Maps to `exchange`
        public string? Type { get; set; } // Maps to `type` (optional string)
        public string? Instrument { get; set; } // Maps to `instrument` (optional string)
        public string? OptionType { get; set; } // Maps to `optionType` (optional string)
        public string? ExpiryDate { get; set; } // Maps to `ExpiryDate` (nullable string)
        public string CallStrikePrice { get; set; } // Maps to `CallStrikePrice`
        public string PutStrikePrice { get; set; } // Maps to `PutStrikePrice`
        public string Direction { get; set; }
        public int NoOfLots { get; set; } // Maps to `NoOfLots`
        public int Multiple { get; set; } // Maps to `Multiple`
        public int Status { get; set; } // Maps to `Status`
    }
}
