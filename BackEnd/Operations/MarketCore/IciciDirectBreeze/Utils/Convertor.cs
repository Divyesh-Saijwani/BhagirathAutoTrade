using MarketData.Entities;
using MarketDataSync.Models.DTOs;
using System.Globalization;
using System.Text.Json;

namespace IciciDirectBreeze.Utils
{
    public static class Convertor
    {
        public static MarketPriceResponse ToMarketPriceResponse(this Dictionary<string, object> jsonDict)
        {
            MarketPriceResponse response = new MarketPriceResponse();

            // Extract Success field
            if (jsonDict.TryGetValue("Success", out var successObject))
            {
                // Extract Status field
                if (jsonDict.TryGetValue("Status", out var statusObject) && int.TryParse(statusObject.ToString(), out var status))
                {
                    response.Status = status;
                }

                // Extract Error field
                if (jsonDict.TryGetValue("Error", out var errorObject))
                {
                    response.Error = errorObject?.ToString();
                }
                response.Success = new List<MarketPriceData>();
                try
                {

                    if (response.Error==null || !response.Error.Equals("No Data Found"))
                    {


                        var successList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(successObject.ToString());

                        foreach (var item in successList)
                        {
                            if (item is Dictionary<string, object> itemDict)
                            {
                                MarketPriceData data = new MarketPriceData();
                                if (itemDict.TryGetValue("datetime", out var datetime) && DateTime.TryParse(datetime.ToString(), out var parsedDateTime))
                                {
                                    data.DateTime = parsedDateTime;
                                    data.UploadDate = parsedDateTime;
                                }

                                // Map other properties with null checks and parsing
                                if (itemDict.TryGetValue("stock_code", out var stockCode))
                                    data.StockCode = stockCode?.ToString();
                                if (itemDict.TryGetValue("exchange_code", out var exchangeCode))
                                    data.ExchangeCode = exchangeCode?.ToString();
                                if (itemDict.TryGetValue("expiry_date", out var expiryDate) && DateTime.TryParse(expiryDate?.ToString(), out var parsedExpiryDate))
                                    data.ExpiryDate = parsedExpiryDate;
                                if (itemDict.TryGetValue("right", out var right))
                                    data.Right = right?.ToString();
                                if (itemDict.TryGetValue("strike_price", out var strikePrice) && decimal.TryParse(strikePrice?.ToString(), out var parsedStrikePrice))
                                    data.StrikePrice = parsedStrikePrice;
                                if (itemDict.TryGetValue("open", out var open) && decimal.TryParse(open?.ToString(), out var parsedOpen))
                                    data.Open = parsedOpen;
                                if (itemDict.TryGetValue("high", out var high) && decimal.TryParse(high?.ToString(), out var parsedHigh))
                                    data.High = parsedHigh;
                                if (itemDict.TryGetValue("low", out var low) && decimal.TryParse(low?.ToString(), out var parsedLow))
                                    data.Low = parsedLow;
                                if (itemDict.TryGetValue("close", out var close) && decimal.TryParse(close?.ToString(), out var parsedClose))
                                    data.Close = parsedClose;
                                if (itemDict.TryGetValue("volume", out var volume) && long.TryParse(volume?.ToString(), out var parsedVolume))
                                    data.Volume = parsedVolume;
                                if (itemDict.TryGetValue("open_interest", out var openInterest) && long.TryParse(openInterest?.ToString(), out var parsedOpenInterest))
                                    data.OpenInterest = parsedOpenInterest;
                                if (itemDict.TryGetValue("count", out var count) && int.TryParse(count?.ToString(), out var parsedCount))
                                    data.Count = parsedCount;
                                data.OptionType = (bool)(right?.ToString().ToLower().Equals("others")) ? "XX" : ((bool)(right?.ToString().ToLower().Equals("call")) ? "CE" : "PE");
                                data.FileType = data.OptionType.Equals("XX") ? 8 : 11;
                                response.Success.Add(data);
                            }
                        }
                    }
                }
                catch
                {

                }
            }



            return response;
        }

        public static ApiLiveOptionDataResponse ConvertToApiLiveOptionDataResponse(this Dictionary<string, object> dict)
        {
            var response = new ApiLiveOptionDataResponse();

            // Extract Success field
            if (dict.TryGetValue("Success", out var successObject))
            {
                // Extract Status field
                if (dict.TryGetValue("Status", out var statusObject) && int.TryParse(statusObject.ToString(), out var status))
                {
                    response.Status = status;
                }

                // Extract Error field
                if (dict.TryGetValue("Error", out var errorObject))
                {
                    response.Error = errorObject;
                }

                try
                {

                    if (response.Error == null || !response.Error.Equals("No Data Found"))
                    {
                        var successList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(successObject.ToString());
                        response.Success = new List<LiveOptionData>();

                        foreach (var item in successList)
                        {
                            if (item is Dictionary<string, object> itemDict)
                            {
                                var data = new LiveOptionData();

                                data.ExchangeCode = itemDict.TryGetValue("exchange_code", out var exchangeCode) ? exchangeCode.ToString() : null;
                                data.ProductType = itemDict.TryGetValue("product_type", out var productType) ? productType.ToString() : null;
                                data.StockCode = itemDict.TryGetValue("stock_code", out var stockCode) ? stockCode.ToString() : null;
                                data.Right = itemDict.TryGetValue("right", out var right) ? right.ToString() : null;
                                data.StrikePrice = itemDict.TryGetValue("strike_price", out var strikePrice) && double.TryParse(strikePrice?.ToString(), out var parsedStrikePrice) ? parsedStrikePrice : 0.0;
                                data.Ltp = itemDict.TryGetValue("ltp", out var ltp) && double.TryParse(ltp?.ToString(), out var parsedltp) ? parsedltp : 0.0;
                                data.Open = itemDict.TryGetValue("open", out var open) && double.TryParse(open?.ToString(), out var parsedopen) ? parsedopen : 0.0;
                                data.High = itemDict.TryGetValue("high", out var high) && double.TryParse(high?.ToString(), out var parsedhigh) ? parsedhigh : 0.0;
                                data.Low = itemDict.TryGetValue("low", out var low) && double.TryParse(low?.ToString(), out var parsedlow) ? parsedlow : 0.0;
                                data.PreviousClose = itemDict.TryGetValue("previous_close", out var previousClose) && double.TryParse(previousClose?.ToString(), out var parsedpreviousClose) ? parsedpreviousClose : 0.0;
                                data.LtpPercentChange = itemDict.TryGetValue("ltp_percent_change", out var ltpPercentChange) && double.TryParse(ltpPercentChange?.ToString(), out var parsedltpPercentChange) ? parsedltpPercentChange : 0.0;


                                response.Success.Add(data);
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            

            return response;
        }
    }
}
