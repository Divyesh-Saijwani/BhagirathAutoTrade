using AlgoBhagirath.Common.Enums;
using AlgoBhagirath.Common.Utils;
using BhagirathFincareClassLibrary.Models;
using BhagirathFincareCore.Lib;
using Breeze;
using IciciDirectBreeze.Models;
using IciciDirectBreeze.Utils;
using MarketData.Entities;
using MarketData.Services;
using MarketDataSync.Models.DTOs;
using System.Globalization;

namespace IciciDirectBreeze.Services
{
    public class MarketDataService : IMarketData
    {
        private readonly IciciDirectBreezeConfiguration _config;
        private readonly BreezeConnect _breeze;

        public MarketDataService(IciciDirectBreezeConfiguration config)
        {
            _config = config;

            // Initialize SDK 
            _breeze = new BreezeConnect(_config.AppKey);
        }

        public bool CheckTokenStatusToken(string token)
        {
            _config.SessionToken = token;
            var result = this.GetCustomerDetails();

            if (result !=null && result.TryGetValue("Error", out var errorObject))
            {
                return !string.Equals(errorObject?.ToString(), "Session not generated. Please generate session.");
            }
            
            return false;
        }

        //public Dictionary<string, object> GetEquitySymbols(string exchange)
        //{

        //    // Generate Session
        //    _breeze.generateSession(_config.AppSecret, _config.SessionToken);

        //    // Get Historical Data for specific stock-code by mentioned interval either as "minute", "5minute", "30minutes" or as "day".
        //    return _breeze.
        //}



        public MarketPriceResponse GetEquityDataByDate(string exchange, string symbol, DateTime fromDate, DateTime toDate, string interval = "day")
        {

            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            // Get Historical Data for specific stock-code by mentioned interval either as "minute", "5minute", "30minutes" or as "day".
            return _breeze.getHistoricalData(interval: interval, fromDate: fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), toDate: toDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), stockCode: symbol, exchangeCode: exchange, productType: "equity", expiryDate: "", right: "", strikePrice: "").ToMarketPriceResponse();
        }

        public MarketPriceResponse GetDerivitiveDataByDate(string exchange, string symbol, string product, string expiryDate, string optionType, int strikePrice, DateTime? fromDate, DateTime? toDate, string interval = "day")
        {

            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            var right= (optionType.ToString().ToLower().Equals("xx")) ? "others" : ((optionType.ToString().ToLower().Equals("ce")) ? "call" : "put");

            // Get Historical Data for specific stock-code by mentioned interval either as "minute", "5minute", "30minutes" or as "day".
            return _breeze.getHistoricalData(interval: interval, fromDate: (fromDate ?? DateTime.Today).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), toDate: (toDate ?? DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), stockCode: GetFOStockCode(symbol), exchangeCode: "NFO", productType: product, expiryDate: expiryDate, right: right, strikePrice: strikePrice.ToString()).ToMarketPriceResponse();
        }

        public Dictionary<string, object> GetCustomerDetails()
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            return _breeze.getCustomerDetail(_config.SessionToken);
        }

        public Dictionary<string, object> GetFunds()
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            return _breeze.getFunds();
        }


        public Dictionary<string,object> SetFunds(string transactionType,double amount, string segment)
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            return _breeze.setFunds(transactionType,amount.ToString(),segment);
        }

        public Dictionary<string, object> GetDematHoldings()
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            return _breeze.getDematHoldings();
        }

        public ApiLiveOptionDataResponse GetLiveFutureData(string symbol,DateTime expiry)
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            return _breeze.getQuotes(GetFOStockCode(symbol),"NFO",expiry.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), "futures", "others","0").ConvertToApiLiveOptionDataResponse();
        }

        public ApiLiveOptionDataResponse GetLiveOptionData(string symbol, string optionType, DateTime expiry, string strikePrice)
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);
            var right = optionType.ToString().ToLower().Equals("ce") ? "call" : "put";

            return _breeze.getOptionChainQuotes(GetFOStockCode(symbol), "NFO", expiry.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), "options", right, strikePrice).ConvertToApiLiveOptionDataResponse(); ;
        }

        public IEnumerable<int> GetOptionsStrikePrices(string symbol)
        {
            var futureExpiries = new List<DateTime>();
            var optionExpiries = new List<DateTime>();
            var stockCode = GetFOStockCode(symbol);
            switch (symbol.ToLower())
            {
                case "nifty":
                    futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Thursday);
                    optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Thursday);
                    break;

                case "banknifty":
                case "cnxban":
                    futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Wednesday);
                    optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Wednesday);
                    break;

                case "finnifty":
                case "niffin":
                    futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Tuesday);
                    optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Tuesday);
                    break;
            }
            var futureLatestData = this.GetLiveFutureData(stockCode, futureExpiries.FirstOrDefault());
            // Get the first item with Ltp > 0
            var firstItem = futureLatestData.Success.FirstOrDefault(x => x.Ltp > 0);

            // Convert Ltp to int with rounding
            int ltpAsInt = firstItem != null ? Convert.ToInt32(Math.Round(firstItem.Ltp)) : 0;

            // Generate values around the base
            var strikePrices = NumberUtils.GenerateValuesAroundBase(ltpAsInt);

            return strikePrices;
        }

        public IEnumerable<DateTime> GetExpiryDates(string symbol, string optionType)
        {
            var expiries = new List<DateTime>();
            switch (symbol.ToLower())
            {
                case "nifty":
                    expiries = optionType.ToLower().Equals("xx") ? DateTime.Now.GetLastDayOfMonth(DayOfWeek.Thursday) : DateTime.Now.GetNextDayOfWeek(DayOfWeek.Thursday);
                    break;

                case "banknifty":
                    expiries = optionType.ToLower().Equals("xx") ? DateTime.Now.GetLastDayOfMonth(DayOfWeek.Wednesday) : DateTime.Now.GetNextDayOfWeek(DayOfWeek.Wednesday);
                    break;

                case "finnifty":
                    expiries = optionType.ToLower().Equals("xx") ?  DateTime.Now.GetLastDayOfMonth(DayOfWeek.Tuesday) : DateTime.Now.GetNextDayOfWeek(DayOfWeek.Tuesday);
                    break;
            }
            return expiries;
        }



        public async Task<object> GetCalculatedData(string symbol, string exchange, string type, string instrument, string expiryDate, string date)
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);
            var interval = string.Empty;


            var futureHistory = this.GetLiveFutureData(this.GetFOStockCode(symbol), DateTime.ParseExact(expiryDate, "MM/dd/yyyy", new CultureInfo("en-GB"))).Success.LastOrDefault();
            // _breeze.getHistoricalData(interval, workingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), workingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), symbol, exchange, type, DateTime.Parse(expiryDate).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), "Others", "0").ToMarketPriceResponse().Success.LastOrDefault();
            var calculateFuture = new DtoRequestModelForCalculate();
            calculateFuture.Symbole = symbol;
            calculateFuture.Exchange = exchange;
            calculateFuture.Type = type;
            calculateFuture.Instrument = instrument;
            calculateFuture.ExpiryDate = expiryDate;
            calculateFuture.WorkingDate = date;
            calculateFuture.OptionType = "XX";
            calculateFuture.CMP = Convert.ToDecimal(futureHistory.Ltp);
            calculateFuture.Close = Convert.ToDecimal(futureHistory.PreviousClose);
            calculateFuture.Open = Convert.ToDecimal(futureHistory.Open);
            calculateFuture.IDH = Convert.ToDecimal(futureHistory.High);
            calculateFuture.IDL = Convert.ToDecimal(futureHistory.Low);
            calculateFuture.Average = (calculateFuture.IDH + calculateFuture.IDL) / 2;
            
            string message = "";
            EquityLib equitylib = new EquityLib();
            var response = equitylib.CalculateEquity(calculateFuture, out message);

            return response;
        }

        private string GetFOStockCode(string symbol)
        {
            var stockCode = "";
            switch (symbol.ToLower())
            {
                case "nifty":
                    stockCode = "NIFTY";
                    break;

                case "banknifty":
                    stockCode = "CNXBAN";
                    break;

                case "finnifty":
                    stockCode = "NIFFIN";
                    break;
            }
            return stockCode;
        }

    }
}
