using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketDataSync.Models.DTOs
{
    public class ApiLiveOptionDataResponse
    {
        [JsonProperty("Success")]
        public List<LiveOptionData> Success { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("Error")]
        public object Error { get; set; }
    }
    public class LiveOptionData
    {
        [JsonProperty("exchange_code")]
        public string ExchangeCode { get; set; }

        [JsonProperty("product_type")]
        public string ProductType { get; set; }

        [JsonProperty("stock_code")]
        public string StockCode { get; set; }

        [JsonProperty("expiry_date")]
        public string ExpiryDate { get; set; }

        [JsonProperty("right")]
        public string Right { get; set; }

        [JsonProperty("strike_price")]
        public double StrikePrice { get; set; }

        [JsonProperty("ltp")]
        public double Ltp { get; set; }

        [JsonProperty("ltt")]
        public string Ltt { get; set; }

        [JsonProperty("best_bid_price")]
        public double BestBidPrice { get; set; }

        [JsonProperty("best_bid_quantity")]
        public string BestBidQuantity { get; set; }

        [JsonProperty("best_offer_price")]
        public double BestOfferPrice { get; set; }

        [JsonProperty("best_offer_quantity")]
        public string BestOfferQuantity { get; set; }

        [JsonProperty("open")]
        public double Open { get; set; }

        [JsonProperty("high")]
        public double High { get; set; }

        [JsonProperty("low")]
        public double Low { get; set; }

        [JsonProperty("previous_close")]
        public double PreviousClose { get; set; }

        [JsonProperty("ltp_percent_change")]
        public double LtpPercentChange { get; set; }

        [JsonProperty("upper_circuit")]
        public double UpperCircuit { get; set; }

        [JsonProperty("lower_circuit")]
        public double LowerCircuit { get; set; }

        [JsonProperty("total_quantity_traded")]
        public string TotalQuantityTraded { get; set; }

        [JsonProperty("spot_price")]
        public string SpotPrice { get; set; }

        [JsonProperty("ltq")]
        public string Ltq { get; set; }

        [JsonProperty("open_interest")]
        public double OpenInterest { get; set; }

        [JsonProperty("chnge_oi")]
        public double ChangeOi { get; set; }

        [JsonProperty("total_buy_qty")]
        public string TotalBuyQty { get; set; }

        [JsonProperty("total_sell_qty")]
        public string TotalSellQty { get; set; }
    }
}