using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketDataSync.Models.DTOs
{
    public class StockFutureData
    {
        [JsonProperty("datetime")]
        public DateTime Datetime { get; set; }

        [JsonProperty("stock_code")]
        public string StockCode { get; set; }

        [JsonProperty("exchange_code")]
        public string ExchangeCode { get; set; }

        [JsonProperty("product_type")]
        public string ProductType { get; set; }

        [JsonProperty("expiry_date")]
        public string ExpiryDate { get; set; }

        [JsonProperty("right")]
        public string Right { get; set; }

        [JsonProperty("strike_price")]
        public string StrikePrice { get; set; }

        [JsonProperty("open")]
        public string Open { get; set; }

        [JsonProperty("high")]
        public string High { get; set; }

        [JsonProperty("low")]
        public string Low { get; set; }

        [JsonProperty("close")]
        public string Close { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

        [JsonProperty("open_interest")]
        public object OpenInterest { get; set; }  // Can be `null` or a number
    }
}