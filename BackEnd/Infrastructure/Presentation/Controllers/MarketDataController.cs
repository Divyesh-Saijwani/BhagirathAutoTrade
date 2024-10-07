using MarketData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Services.Abstractions;
using Contracts.Calculation;
using AlgoBhagirath.Common.Utils;
//using MarketDataSync.Models;
//using MarketDataSync.Models.DTOs;
using MarketData.Entities;
using Mapster;
using BhagirathFincareClassLibrary.Models;
using MarketDataSync.Models.DTOs;
using System.Globalization;
using AlgoBhagirath.Common.Enums;
using Contracts;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/MarketData")]
    public class MarketDataController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMarketData _marketData;
        public MarketDataController(IMarketData marketData, IServiceManager serviceManager)
        {
            _marketData = marketData;
            _serviceManager = serviceManager;
        }


        [HttpGet]
        public async Task<List<MarketPriceData>> GetMarketData(string exchange, string symbol, string product, string expiryDate, string optionType, int strikePrice, DateTime? fromDate, DateTime? toDate, string interval)
        {
            if (product == null)
            {
                return _marketData.GetEquityDataByDate(exchange, symbol, fromDate ?? DateTime.Today, toDate ?? DateTime.Now, interval).Success;
            }
            else
            {
                return _marketData.GetDerivitiveDataByDate(exchange, symbol, product, expiryDate, optionType, strikePrice, fromDate ?? DateTime.Today, toDate ?? DateTime.Now, interval).Success;
            }
        }

        [HttpGet("GetCustomerData")]
        public async Task<Dictionary<string, object>> GetCustomerData()
        {
            return _marketData.GetCustomerDetails();
        }

        [HttpGet("GetFunds")]
        public async Task<Dictionary<string, object>> GetFunds()
        {
            return _marketData.GetFunds();
        }

        [HttpGet("GetDematHoldings")]
        public async Task<Dictionary<string, object>> GetDematHoldings()
        {
            return _marketData.GetDematHoldings();
        }

        [HttpGet("GetLiveFutureData")]
        public async Task<LiveOptionData> GetLiveFutureData(string symbol, DateTime expiry)
        {
            return _marketData.GetLiveFutureData(symbol, expiry).Success.FirstOrDefault(x => x.Ltp > 0);
        }

        [HttpGet("GetLiveOptionData")]
        public async Task<List<LiveOptionData>> GetLiveOptionData(string symbol, string optionType, DateTime expiry, string strikePrice = "")
        {
            return _marketData.GetLiveOptionData(symbol, optionType, expiry, strikePrice).Success;
        }

        [HttpGet("GetOptionsStrikePrices")]
        public async Task<IEnumerable<int>> GetOptionsStrikePrices(string symbol)
        {
            return _marketData.GetOptionsStrikePrices(symbol);
        }

        [HttpGet("GetExpiryDates")]
        public async Task<IEnumerable<DateTime>> GetExpiryDates(string symbol, string optionType)
        {
            return _marketData.GetExpiryDates(symbol, optionType);
        }

        [HttpGet("Calculate")]
        public async Task<object> Calculate(string exchange, string symbol, string type, string expiryDate, string optionType, int strikePrice, string workingdate, decimal ss, string sst, decimal rs, string rst, decimal hr, decimal hs)
        {
            var futureExpiries = new List<DateTime>();
            var optionExpiries = new List<DateTime>();
            switch (symbol.ToLower())
            {
                case "nifty":
                    futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Thursday);
                    optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Thursday);
                    break;

                case "banknifty":
                    futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Wednesday);
                    optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Wednesday);
                    break;

                case "finnifty":
                    futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Tuesday);
                    optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Tuesday);
                    break;
            }

            var data = await _marketData.GetCalculatedData(symbol, exchange, type, "FUTIDX", expiryDate, workingdate);
            var result = data.Adapt<EquityDetailedData>();
            result.SST = sst;
            result.SS = ss;
            result.RS = rs;
            result.RST = rst;
            result.HS = hs;
            result.HR = hr;

            var selectedExpiry = DateTime.ParseExact(expiryDate, "MM/dd/yyyy", new CultureInfo("en-GB"));
            futureExpiries.Remove(selectedExpiry);
            var liveFuture = _marketData.GetLiveFutureData(symbol, selectedExpiry).Success.FirstOrDefault(x => x.Ltp > 0);
            result.CMP = (decimal)liveFuture.Ltp;

            var futureResult = UpdateCalculationData(result);

            var buyFutureReference = futureResult.FirstOrDefault(x => x.Direction.Equals("BUY"));
            var sellFutureReference = futureResult.FirstOrDefault(x => x.Direction.Equals("SELL"));

            if (TimeSpan.TryParse(sst, out TimeSpan bTime))
            {
                // Combine today's date with the time
                buyFutureReference.EntryTime = DateTime.Today.Add(bTime);
            }

            if (TimeSpan.TryParse(rst, out TimeSpan aTime))
            {
                // Combine today's date with the time
                sellFutureReference.EntryTime = DateTime.Today.Add(aTime);
            }
            buyFutureReference.Expiry = selectedExpiry;
            buyFutureReference.Symbol = symbol;
            buyFutureReference.Exchange = "NSE";
            buyFutureReference.OptionType = "XX";
            buyFutureReference.Type = "DERIVATIVE";
            buyFutureReference.Instrument = "FUTIDX";

            sellFutureReference.Expiry = selectedExpiry;
            sellFutureReference.Symbol = symbol;
            sellFutureReference.Exchange = "NSE";
            sellFutureReference.OptionType = "XX";
            sellFutureReference.Type = "DERIVATIVE";
            sellFutureReference.Instrument = "FUTIDX";


            var calculationData = new List<CalculationDataDto>();


            foreach (var item in futureExpiries)
            {
                var buyData = new CalculationDataDto { Direction = "BUY", Expiry = item, Symbol = symbol, Exchange = "NSE", OptionType = "XX", Type = "DERIVATIVE", Instrument = "FUTIDX" };
                var sellData = new CalculationDataDto { Direction = "SELL", Expiry = item, Symbol = symbol, Exchange = "NSE", OptionType = "XX", Type = "DERIVATIVE", Instrument = "FUTIDX" };
                var liveData = _marketData.GetLiveFutureData(symbol, item).Success.FirstOrDefault(x => x.Ltp > 0);


                buyData.Trend = result.txt_K13_not.ToUpper();
                sellData.Trend = result.txt_K13_not.ToUpper();

                if (buyFutureReference.LowPoint > 0)
                {
                    buyData.LowPoint = (decimal)liveData.Ltp - (result.CMP - buyFutureReference.LowPoint);
                }
                if (buyFutureReference.AveragePoint > 0)
                {
                    buyData.AveragePoint = (decimal)liveData.Ltp - (result.CMP - buyFutureReference.AveragePoint);
                }
                if (buyFutureReference.MaxPoint > 0)
                {
                    buyData.MaxPoint = (decimal)liveData.Ltp - (result.CMP - buyFutureReference.MaxPoint);
                }
                buyData.StopLoss = (decimal)liveData.Ltp - (result.CMP - buyFutureReference.StopLoss);
                buyData.EntryTime = buyFutureReference.EntryTime;


                if (sellFutureReference.LowPoint > 0)
                {
                    sellData.LowPoint = (decimal)liveData.Ltp - (result.CMP - sellFutureReference.LowPoint);
                }
                if (sellFutureReference.AveragePoint > 0)
                {
                    sellData.AveragePoint = (decimal)liveData.Ltp - (result.CMP - sellFutureReference.AveragePoint);
                }
                if (sellFutureReference.MaxPoint > 0)
                {
                    sellData.MaxPoint = (decimal)liveData.Ltp - (result.CMP - sellFutureReference.MaxPoint);
                }
                sellData.StopLoss = (result.CMP - sellFutureReference.StopLoss) + (decimal)liveData.Ltp;
                sellData.EntryTime = sellFutureReference.EntryTime;

                // Initialize a list of the points, excluding zero values
                var points = new List<decimal>
                {
                    sellData.LowPoint != 0 ? sellData.LowPoint : decimal.MaxValue,
                    sellData.AveragePoint != 0 ? sellData.AveragePoint : decimal.MaxValue,
                    sellData.MaxPoint != 0 ? sellData.MaxPoint : decimal.MaxValue
                };

                // Find the minimum value among the non-zero points
                buyData.Target = Math.Min(points[0], Math.Min(points[1], points[2]));
                sellData.Target = Math.Max(Math.Max(buyData.LowPoint, buyData.AveragePoint), buyData.MaxPoint);

                calculationData.Add(buyData);
                calculationData.Add(sellData);

            }

            futureResult.AddRange(calculationData);

            await _serviceManager.MarketDataService.InsertTodayCalculationsAsync(futureResult);

            calculationData = new List<CalculationDataDto>();
            var strikePriices = _marketData.GetOptionsStrikePrices(symbol).Where(x => (x < result.CMP - 200) || (x < result.CMP + 200)).ToList();

            foreach (var item in optionExpiries)
            {
                var callLiveData = _marketData.GetLiveOptionData(symbol, "CE", item, "0").Success?.Where(x => x.Ltp > 0 && strikePriices.Contains((int)x.StrikePrice));
                var putLiveData = _marketData.GetLiveOptionData(symbol, "PE", item, "0").Success?.Where(x => x.Ltp > 0 && strikePriices.Contains((int)x.StrikePrice));

                foreach (var price in strikePriices)
                {
                    var callData = callLiveData?.FirstOrDefault(x => x.StrikePrice == price);
                    var putData = putLiveData?.FirstOrDefault(x => x.StrikePrice == price);

                    if (callData != null)
                    {
                        var callBuyData = new CalculationDataDto { Direction = "BUY", Expiry = item, Symbol = symbol, Exchange = "NSE", OptionType = "CE", Type = "DERIVATIVE", Instrument = "OPTIDX" };
                        var callSellData = new CalculationDataDto { Direction = "SELL", Expiry = item, Symbol = symbol, Exchange = "NSE", OptionType = "CE", Type = "DERIVATIVE", Instrument = "OPTIDX" };


                        callBuyData.Trend = result.txt_K13_not.ToUpper();
                        callSellData.Trend = result.txt_K13_not.ToUpper();

                        if (buyFutureReference.LowPoint > 0)
                        {
                            callBuyData.LowPoint = (decimal)callData.Ltp - (result.CMP - buyFutureReference.LowPoint);
                            callBuyData.LowPoint = callBuyData.LowPoint < 0 ? 2 : callBuyData.LowPoint;
                        }
                        if (buyFutureReference.AveragePoint > 0)
                        {
                            callBuyData.AveragePoint = (decimal)callData.Ltp - (result.CMP - buyFutureReference.AveragePoint);
                            callBuyData.AveragePoint = callBuyData.AveragePoint < 0 ? 2 : callBuyData.AveragePoint;
                        }
                        if (buyFutureReference.MaxPoint > 0)
                        {
                            callBuyData.MaxPoint = (decimal)callData.Ltp - (result.CMP - buyFutureReference.MaxPoint);
                            callBuyData.MaxPoint = callBuyData.MaxPoint < 0 ? 2 : callBuyData.MaxPoint;
                        }


                        callBuyData.StopLoss = (decimal)callData.Ltp - (result.CMP - buyFutureReference.StopLoss);
                        callBuyData.StopLoss = callBuyData.StopLoss < 0 ? 1 : callBuyData.StopLoss;
                        callBuyData.EntryTime = buyFutureReference.EntryTime;
                        callBuyData.StrikePrice = price;


                        if (sellFutureReference.LowPoint > 0)
                        {
                            callSellData.LowPoint = (sellFutureReference.LowPoint - result.CMP) + (decimal)callData.Ltp;
                        }
                        if (sellFutureReference.AveragePoint > 0)
                        {
                            callSellData.AveragePoint = (sellFutureReference.AveragePoint - result.CMP) + (decimal)callData.Ltp;
                        }
                        if (sellFutureReference.MaxPoint > 0)
                        {
                            callSellData.MaxPoint = (sellFutureReference.MaxPoint - result.CMP) + (decimal)callData.Ltp;
                        }
                        callSellData.StopLoss = (sellFutureReference.StopLoss - result.CMP) + (decimal)callData.Ltp;
                        callSellData.EntryTime = sellFutureReference.EntryTime;
                        callSellData.StrikePrice = price;

                        // Initialize a list of the points, excluding zero values
                        var callSellDataPoints = new List<decimal>
                        {
                            callSellData.LowPoint != 0 ? callSellData.LowPoint : decimal.MaxValue,
                            callSellData.AveragePoint != 0 ? callSellData.AveragePoint : decimal.MaxValue,
                            callSellData.MaxPoint != 0 ? callSellData.MaxPoint : decimal.MaxValue
                        };

                        // Find the minimum value among the non-zero points
                        callBuyData.Target = Math.Min(callSellDataPoints[0], Math.Min(callSellDataPoints[1], callSellDataPoints[2]));
                        callSellData.Target = Math.Max(Math.Max(callBuyData.LowPoint, callBuyData.AveragePoint), callBuyData.MaxPoint);

                        calculationData.Add(callBuyData);
                        calculationData.Add(callSellData);

                    }

                    if (putData != null)
                    {
                        var putBuyData = new CalculationDataDto { Direction = "BUY", Expiry = item, Symbol = symbol, Exchange = "NSE", OptionType = "PE", Type = "DERIVATIVE", Instrument = "OPTIDX" };
                        var putSellData = new CalculationDataDto { Direction = "SELL", Expiry = item, Symbol = symbol, Exchange = "NSE", OptionType = "PE", Type = "DERIVATIVE", Instrument = "OPTIDX" };

                        putBuyData.Trend = result.txt_K13_not.ToUpper().Equals("BUY") ? "SELL" : "BUY";
                        putSellData.Trend = result.txt_K13_not.ToUpper().Equals("BUY") ? "SELL" : "BUY";

                        if (buyFutureReference.LowPoint > 0)
                        {
                            putBuyData.LowPoint = (decimal)putData.Ltp - (sellFutureReference.LowPoint - result.CMP);
                            putBuyData.LowPoint = putBuyData.LowPoint < 0 ? 2 : putBuyData.LowPoint;
                        }
                        if (buyFutureReference.AveragePoint > 0)
                        {
                            putBuyData.AveragePoint = (decimal)putData.Ltp - (sellFutureReference.AveragePoint - result.CMP);
                            putBuyData.AveragePoint = putBuyData.AveragePoint < 0 ? 2 : putBuyData.AveragePoint;
                        }
                        if (buyFutureReference.MaxPoint > 0)
                        {
                            putBuyData.MaxPoint = (decimal)putData.Ltp - (sellFutureReference.MaxPoint - result.CMP);
                            putBuyData.MaxPoint = putBuyData.MaxPoint < 0 ? 2 : putBuyData.MaxPoint;
                        }
                        putBuyData.StopLoss = (decimal)putData.Ltp - (sellFutureReference.StopLoss - result.CMP);
                        putBuyData.StopLoss = putBuyData.StopLoss<0?1:putBuyData.StopLoss;
                        putBuyData.EntryTime = buyFutureReference.EntryTime;
                        putBuyData.StrikePrice = price;

                        if (sellFutureReference.LowPoint > 0)
                        {
                            putSellData.LowPoint = (result.CMP - buyFutureReference.LowPoint) + (decimal)putData.Ltp;
                        }
                        if (sellFutureReference.AveragePoint > 0)
                        {
                            putSellData.AveragePoint = (result.CMP - buyFutureReference.AveragePoint) + (decimal)putData.Ltp;
                        }
                        if (sellFutureReference.MaxPoint > 0)
                        {
                            putSellData.MaxPoint = (result.CMP - buyFutureReference.MaxPoint) + (decimal)putData.Ltp;
                        }
                        putSellData.StopLoss = (result.CMP - buyFutureReference.StopLoss) + (decimal)putData.Ltp;
                        putSellData.EntryTime = sellFutureReference.EntryTime;
                        putSellData.StrikePrice = price;

                        // Initialize a list of the points, excluding zero values
                        var putSellDataPoints = new List<decimal>
                        {
                            putSellData.LowPoint != 0 ? putSellData.LowPoint : decimal.MaxValue,
                            putSellData.AveragePoint != 0 ? putSellData.AveragePoint : decimal.MaxValue,
                            putSellData.MaxPoint != 0 ? putSellData.MaxPoint : decimal.MaxValue
                        };

                        // Find the minimum value among the non-zero points
                        putBuyData.Target = Math.Min(putSellDataPoints[0], Math.Min(putSellDataPoints[1], putSellDataPoints[2]));
                        putSellData.Target = Math.Max(Math.Max(putBuyData.LowPoint, putBuyData.AveragePoint), putBuyData.MaxPoint);

                        calculationData.Add(putBuyData);
                        calculationData.Add(putSellData);

                    }
                }
            }


            await _serviceManager.MarketDataService.InsertTodayCalculationsAsync(calculationData);

            return futureResult;
        }

        [HttpGet("GetTodayCalculations")]
        public async Task<IEnumerable<CalculationDataDto>> GetTodayCalculationsAsync()
        {
            return await _serviceManager.MarketDataService.GetTodayCalculationsAsync();
        }

        [HttpPost("SyncMarketData")]
        public async Task<ActionResult> SyncMarketData(string symbol)
        {
            try
            {
                var marketData = new List<MarketPriceData>();

                // Generate Session

                var futureExpiries = new List<DateTime>();
                var optionExpiries = new List<DateTime>();
                switch (symbol.ToLower())
                {
                    case "nifty":
                        futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Thursday);
                        optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Thursday);
                        break;

                    case "banknifty":
                        futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Wednesday);
                        optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Wednesday);
                        break;

                    case "finnifty":
                        futureExpiries = DateTime.Now.GetLastDayOfMonth(DayOfWeek.Tuesday);
                        optionExpiries = DateTime.Now.GetNextDayOfWeek(DayOfWeek.Tuesday);
                        break;
                }

                var fromDate = DateTime.Now.CalculateWorkingDaysBefore(45);
                var toDate = DateTime.Now;


                foreach (var date in futureExpiries)
                {
                    var result = _marketData.GetDerivitiveDataByDate(exchange: "NFO", symbol: symbol, product: "futures", expiryDate: date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), optionType: "XX", strikePrice: 0, fromDate: fromDate, toDate: toDate, interval: "day");
                    marketData.AddRange(result.Success.Adapt<List<MarketPriceData>>());
                }

                //var latestData = _marketData.GetLiveFutureData(symbol, futureExpiries.FirstOrDefault());

                //var strikePriices = NumberUtils.GenerateValuesAroundBase(Convert.ToInt32(latestData.Success.FirstOrDefault(x => x.Ltp > 0).Ltp));


                //foreach (var date in optionExpiries)
                //{
                //    var expiry = date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                //    foreach (var strikePrice in strikePriices)
                //    {
                //        var callResult = _marketData.GetDerivitiveDataByDate(exchange: "NFO", symbol: symbol, product: "options", expiryDate: expiry, optionType: "ce", strikePrice: strikePrice, fromDate: fromDate, toDate: toDate, interval: "day");
                //        marketData.AddRange(callResult.Success.Adapt<List<MarketPriceData>>());

                //        var putResult = _marketData.GetDerivitiveDataByDate(exchange: "NFO", symbol: symbol, product: "options", expiryDate: expiry, optionType: "pe", strikePrice: strikePrice, fromDate: fromDate, toDate: toDate, interval: "day");
                //        marketData.AddRange(putResult.Success.Adapt<List<MarketPriceData>>());
                //    }
                //}

                if (!string.IsNullOrEmpty(symbol))
                {
                    marketData.ForEach(x => x.Symbol = symbol);
                    await _serviceManager.MarketDataService.InsertMarketPricesAsync(marketData.Adapt<IEnumerable<MarketPrice>>());
                }

                return Ok(marketData);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("GetAlgoConfigurations")]
        public async Task<IActionResult> GetAlgoConfigurations()
        {
            return Ok(await _serviceManager.MarketDataService.GetAlgoConfigurations());
        }

        [HttpPost("InsertAlgoConfigurations")]
        public async Task<IActionResult> InsertAlgoConfigurations(UserAlgoDataDto data)
        {
            return Ok(await _serviceManager.MarketDataService.InsertAlgoConfigurationsAsync(data));
        }

        private List<CalculationDataDto> UpdateCalculationData(EquityDetailedData data)
        {
            var result = new List<CalculationDataDto>();
            var buyData = new CalculationDataDto { Direction = "BUY" };
            var sellData = new CalculationDataDto { Direction = "SELL" };

            var trend = data.txt_K13_not.ToLower();
            var S3Heading = "S3";
            var R2Heading = "R2";
            decimal S3 = 0;
            decimal R2 = 0;
            var Sbap = Math.Round(Convert.ToDecimal(data.txt_J13), 2);
            var Rbap = Math.Round(Convert.ToDecimal(data.txt_N13), 2);
            decimal sellSL = 0;
            decimal buySL = 0;
            decimal entryReferencePoint = Math.Round(data.CMP * 0.0021m, 2);

            switch (trend)
            {
                case "buy":
                    S3 = Math.Round(Convert.ToDecimal(data.txt_J6), 2);
                    R2 = Math.Round(Convert.ToDecimal(data.txt_P6), 2);
                    S3Heading = "S2";
                    R2Heading = "R3";
                    buySL = Math.Round(Convert.ToDecimal(data.txt_d11) - .05m, 2);
                    buyData.Trend = "BUY";
                    sellData.Trend = "BUY";
                    break;

                case "sell":
                    S3 = Math.Round(Convert.ToDecimal(data.txt_I6), 2);
                    R2 = Math.Round(Convert.ToDecimal(data.txt_O6), 2);
                    S3Heading = "S3";
                    R2Heading = "R2";
                    sellSL = Math.Round(Convert.ToDecimal(data.txt_t11) + .05m, 2);
                    buySL = Math.Round(Convert.ToDecimal(data.txt_d11) - .05m, 2);
                    buyData.Trend = "SELL";
                    sellData.Trend = "SELL";
                    break;

                default:
                    S3 = Math.Round(Convert.ToDecimal(data.txt_I6), 2);
                    R2 = Math.Round(Convert.ToDecimal(data.txt_P6), 2);
                    S3Heading = "S3";
                    R2Heading = "R3";
                    Sbap = Math.Round(Convert.ToDecimal(data.txt_I6), 2);
                    Rbap = Math.Round(Convert.ToDecimal(data.txt_P6), 2);
                    sellSL = Math.Round(Convert.ToDecimal(data.txt_t8) + .10m, 2);
                    buySL = Math.Round(Convert.ToDecimal(data.txt_d8) - .10m, 2);
                    buyData.Trend = string.Empty;
                    sellData.Trend = string.Empty;
                    break;
            }

            Sbap = Sbap < 1 ? S3 : Sbap;
            Rbap = Rbap < 1 ? R2 : Rbap;
            try
            {

                var ssrs = Math.Round((data.SS - data.RS) * 0.89m, 2);
                var hsrs = Math.Round((data.HS - data.HR) * 0.89m, 2);
                var srbap = Math.Round((Sbap - Rbap) * 0.89m, 2);
                var s3r2 = Math.Round((S3 - R2) * 0.89m, 2);

                var buyPoints = new List<decimal> { (data.RS + ssrs), (data.HR + hsrs), (Rbap + srbap), (R2 + s3r2) };
                var buyPointMax = buyPoints.Max();
                var buyPointMin = buyPoints.Min();
                var buyPointAverage = (buyPointMax + buyPointMin) / 2;



                var sellPoints = new List<decimal> { (data.SS - ssrs), (data.HS - hsrs), (Sbap - srbap), (S3 - s3r2) };
                var sellPointMax = sellPoints.Max();
                var sellPointMin = sellPoints.Min();
                var sellPointAverage = (sellPointMax + sellPointMin) / 2;


                // Buy Entry Points
                var buyEntryPoints = "";
                //if (entryReferencePoint <= (sellPointMin - buyPointMax))
                {
                    buyEntryPoints = buyEntryPoints + buyPointMax;
                    buyData.MaxPoint = buyPointMax;

                }

                //if (entryReferencePoint <= (sellPointMin - buyPointAverage))
                {
                    var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                    buyEntryPoints = buyEntryPoints + saperator + buyPointAverage;
                    buyData.AveragePoint = buyPointAverage;

                }

                //if (entryReferencePoint <= (sellPointMin - buyPointMin))
                {
                    var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                    buyEntryPoints = buyEntryPoints + saperator + buyPointMin;
                    buyData.LowPoint = buyPointMin;

                }

                // Sell Entry Points
                var sellEntryPoints = "";
                //if (entryReferencePoint <= (sellPointMin - buyPointMax))
                {
                    sellEntryPoints = sellEntryPoints + sellPointMin;
                    sellData.MaxPoint = sellPointMin;

                }

                //if (entryReferencePoint <= (sellPointMin - buyPointAverage))
                {
                    var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                    sellEntryPoints = sellEntryPoints + saperator + sellPointAverage;
                    sellData.AveragePoint = sellPointAverage;

                }

                //if (entryReferencePoint <= (sellPointMin - buyPointMin))
                {
                    var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                    sellEntryPoints = sellEntryPoints + saperator + sellPointMax;
                    sellData.LowPoint = sellPointMax;

                }

                // Target for both Buy and Sell

                buyData.Target = sellPoints.Min(); // Buy target
                sellData.Target = buyPoints.Max(); // Sell target

                switch (trend)
                {
                    case "buy":

                        sellSL = sellPoints.Max() + Math.Round(sellPoints.Max() * .0013m, 2);
                        buySL = Math.Round(Convert.ToDecimal(data.txt_d11) - .05m, 2);
                        break;

                    case "sell":

                        sellSL = Math.Round(Convert.ToDecimal(data.txt_t11) + .05m, 2);
                        buySL = buyPoints.Min() - Math.Round(buyPoints.Min() * .0013m, 2);
                        break;

                    default:

                        sellSL = Math.Round(Convert.ToDecimal(data.txt_t8) + .10m, 2);
                        buySL = Math.Round(Convert.ToDecimal(data.txt_d8) - .10m, 2);
                        break;
                }

                buyData.StopLoss = buySL; // Buy Stop Loss
                sellData.StopLoss = sellSL; // Sell Stop Loss

                result.Add(buyData);
                result.Add(sellData);

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating Excel file: {ex.Message}");
            }

            return result;
        }
    }

}
