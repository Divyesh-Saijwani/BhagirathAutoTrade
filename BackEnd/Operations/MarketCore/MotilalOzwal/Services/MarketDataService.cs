//using BhagirathFincareClassLibrary.Models;
//using MarketData.Services;
//using MotilalOzwal.Models;
//using System.Runtime.InteropServices;

//namespace MotilalOzwal.Services
//{
//    public class MarketDataService : IMarketData
//    {
//        private readonly MotilalOzwalConfiguration _config;
//        //private readonly BreezeConnect _breeze;

//        public MarketDataService(MotilalOzwalConfiguration config)
//        {
//            _config = config;

//            // Initialize SDK 
//            //_breeze = new BreezeConnect(_config.AppKey);
//        }

//        //public Dictionary<string, object> GetEquitySymbols(string exchange)
//        //{

//        //    // Generate Session
//        //    _breeze.generateSession(_config.AppSecret, _config.SessionToken);

//        //    // Get Historical Data for specific stock-code by mentioned interval either as "minute", "5minute", "30minutes" or as "day".
//        //    return _breeze.
//        //}

//        public Dictionary<string,object> GetEquityDataByDate(string exchange, string symbol, DateTime fromDate, DateTime toDate, string interval = "day")
//        {

//            // Generate Session
//            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

//            // Get Historical Data for specific stock-code by mentioned interval either as "minute", "5minute", "30minutes" or as "day".
//            return _breeze.getHistoricalData(interval: interval, fromDate: fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), toDate: toDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), stockCode: symbol, exchangeCode: exchange, productType: "equity", expiryDate: "", right: "", strikePrice: "");
//        }

//        public Dictionary<string, object> GetDerivitiveDataByDate(string exchange, string symbol, string product, string expiryDate, string direction, int strikePrice, DateTime? fromDate, DateTime? toDate, string interval = "day")
//        {

//            // Generate Session
//            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

//            // Get Historical Data for specific stock-code by mentioned interval either as "minute", "5minute", "30minutes" or as "day".
//            return _breeze.getHistoricalData(interval: interval, fromDate: (fromDate ?? DateTime.Today).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), toDate: (toDate??DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), stockCode: symbol, exchangeCode: exchange, productType: "equity", expiryDate: "", right: "", strikePrice: "");
//        }

//        public Dictionary<string, object> GetCustomerDetails()
//        {
//            // Generate Session
//            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

//            return _breeze.getCustomerDetail(_config.SessionToken);
//        }

//        public Dictionary<string, object> GetFunds()
//        {
//            // Generate Session
//            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

//            return _breeze.getFunds();
//        }

//        public Dictionary<string, object> GetDematHoldings()
//        {
//            // Generate Session
//            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

//            return _breeze.getDematHoldings();
//        }

//        public async Task<Dictionary<string, object>> GetLiveStockDataAsync(string symbol)
//        {
//            // Generate Session
//            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

//            return await _breeze.subscribeFeedsAsync(symbol);
//        }

//        public async Task<object> GetCalculatedData(string symbol,string exchange, string type, string instrument, string expiryDate, string date)
//        {
//            // Generate Session
//            _breeze.generateSession(_config.AppSecret, _config.SessionToken);
//            var interval = string.Empty;
//            var workingDate = DateTime.Parse(date);
//            if (DateTime.Now.Date > workingDate)
//            {
//                interval = "day";
//            }
//            else
//            {
//                interval = "minute";
//            }


//            var futureHistory = _breeze.getHistoricalData(interval: "1minute", fromDate: workingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), toDate: workingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), stockCode: symbol, exchangeCode: exchange, productType: "futures", expiryDate: DateTime.Parse(expiryDate).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), right: "others", strikePrice: "0").ToMarketPriceResponse().Success.LastOrDefault();
//            // _breeze.getHistoricalData(interval, workingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), workingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), symbol, exchange, type, DateTime.Parse(expiryDate).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), "Others", "0").ToMarketPriceResponse().Success.LastOrDefault();
//            var calculateFuture = new DtoRequestModelForCalculate();
//            calculateFuture.Exchange = exchange;
//            calculateFuture.Type = type;
//            calculateFuture.Instrument = instrument;
//            calculateFuture.ExpiryDate = expiryDate;
//            calculateFuture.WorkingDate = date;
//            calculateFuture.OptionType = "XX";
//            calculateFuture.CMP = Convert.ToDecimal(futureHistory.Close);
//            calculateFuture.Close = Convert.ToDecimal(futureHistory.Close);
//            calculateFuture.Open = Convert.ToDecimal(futureHistory.Open);
//            calculateFuture.IDH = Convert.ToDecimal(futureHistory.High);
//            calculateFuture.IDL = Convert.ToDecimal(futureHistory.Low);

//            var calculatorLib = new CalculationLib(_config);

//            var futureResult = calculatorLib.CalculateEquity(calculateFuture,out var message);

//            return futureResult;
//        }
//    }    
//}
