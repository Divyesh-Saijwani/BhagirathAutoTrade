using BhagirathFincareClassLibrary.Models;
using MarketData;
using Microsoft.VisualBasic.FileIO;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using Breeze;
using IciciDirectBreeze.Models;
using AlgoBhagirath.Common.Utils;
using MarketData.Entities;
using AlgoBhagirath.Common.Enums;

namespace IciciDirectBreeze.Utils
{
    public class CalculationLib : EquityLib
    {

        private readonly IciciDirectBreezeConfiguration _config;
        private readonly BreezeConnect _breeze;

        public CalculationLib(IciciDirectBreezeConfiguration config)
        {
            _config = config;

            // Initialize SDK 
            _breeze = new BreezeConnect(_config.AppKey);
        }

        #region Equity - Calculate PDH-PDL, TDH-TDL, CTHD-CTLD

        public override PDLPDHSelectedData GetPDLPDHData(DtoRequestModelForCalculate dtoCalculate)
        {
            try
            {
                var workingDate = DateTime.Parse(dtoCalculate.WorkingDate);
                var fromDate=string.Empty;

                // Generate Session
                _breeze.generateSession(_config.AppSecret, _config.SessionToken);
                var marketData=new List<MarketPriceData>();

                if (dtoCalculate.Type.Equals("cash", StringComparison.OrdinalIgnoreCase))
                {
                    fromDate= workingDate.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                }
                else if (dtoCalculate.Type.Equals("futures", StringComparison.OrdinalIgnoreCase))
                {
                    if (dtoCalculate.Instrument.Equals("FUTIDX", StringComparison.OrdinalIgnoreCase) ||
                        dtoCalculate.Instrument.Equals("FUTIVX", StringComparison.OrdinalIgnoreCase) ||
                        dtoCalculate.Instrument.Equals("FUTSTK", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTimeUtil.IsLastThursdayOfMonth(DateTime.Parse(dtoCalculate.WorkingDate)))
                        {
                            fromDate = workingDate.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        }
                        else
                        {
                            fromDate=DateTimeUtil.GetLastThursdayOfMonth(workingDate.AddMonths(-1)).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        }
                    }
                    
                }
                else if (dtoCalculate.Instrument.Equals("OPTIDX", StringComparison.OrdinalIgnoreCase) ||
                    dtoCalculate.Instrument.Equals("OPTSTK", StringComparison.OrdinalIgnoreCase))
                {
                    fromDate = workingDate.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                }

                marketData = _breeze.getHistoricalData("day", fromDate, fromDate, dtoCalculate.Symbole, dtoCalculate.Exchange, dtoCalculate.Type, dtoCalculate.ExpiryDate, "others", "0").ToMarketPriceResponse().Success;

                var data = marketData.FirstOrDefault();

                var pdlpdhSelectedData = new PDLPDHSelectedData();
                pdlpdhSelectedData.CompanyName = dtoCalculate.Symbole;
                pdlpdhSelectedData.PDH = data.High;
                pdlpdhSelectedData.PDL = data.Low;
                pdlpdhSelectedData.SelectedDateForPDHAndPDL = DateTime.Parse(fromDate);

                return pdlpdhSelectedData;
            }
            catch
            {
            }
            return null;
        }

        public override TDLTDHSelectedData GetTDLTDHData(DtoRequestModelForCalculate dtoCalculate, decimal pdl, decimal pdh, DateTime pdlpdhDate, string companyName)
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            var expirydate = DateTime.Parse(dtoCalculate.ExpiryDate);
            var workingDate= DateTime.Parse(dtoCalculate.WorkingDate);

            List<MarketPriceData> marketPrices = null;

            // Filter based on criteria similar to your stored procedure
            if (expirydate == workingDate)
            {
                marketPrices = _breeze.getHistoricalData("day", DateTime.Parse(dtoCalculate.WorkingDate).AddDays(-1).CalculateWorkingDaysBefore(45).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTime.Parse(dtoCalculate.WorkingDate).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), 
                    dtoCalculate.Symbole, dtoCalculate.Exchange, dtoCalculate.Type, dtoCalculate.ExpiryDate, "others", "0").ToMarketPriceResponse().Success
                    .OrderByDescending(m => m.DateTime).ToList();
            }
            else if (expirydate > workingDate)
            {
                DateTime lastExpiredDate = DateTimeUtil.GetLastThursdayOfMonth(workingDate);

                // Logic to fetch other necessary dates, similar to SQL queries
                // Example: secondLastExpiredDate, prevExpiryDate, secondPrevExpiryDate

                // Simulating printing out variables
                Console.WriteLine($"@ExpiryDate: {expirydate}");
                Console.WriteLine($"@LastExpiredDate: {lastExpiredDate}");
                // Print other dates as needed

                // Filter market prices based on the retrieved dates
                marketPrices = _breeze.getHistoricalData("day", DateTime.Parse(dtoCalculate.WorkingDate).AddDays(-1).CalculateWorkingDaysBefore(45).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTime.Parse(dtoCalculate.WorkingDate).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), 
                    dtoCalculate.Symbole, dtoCalculate.Exchange, dtoCalculate.Type, lastExpiredDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), "others", "0").ToMarketPriceResponse().Success
                    .OrderByDescending(m => m.DateTime).ToList();

            }
            else
            {
                marketPrices = new List<MarketPriceData>();
            }

            // Return top N records based on @NoOfDayOfDataToBeTaken
            var data = marketPrices.Take(45).ToList();

            if (data != null && data.Count > 0)
            {
                var tdltdhSelectedData = new TDLTDHSelectedData();
                foreach (var marketPriceData in data)
                {
                    decimal highValue = Convert.ToDecimal(marketPriceData.High);
                    decimal lowValue = Convert.ToDecimal(marketPriceData.Low);

                    if (highValue > pdh || lowValue < pdl)
                    {
                        tdltdhSelectedData = new TDLTDHSelectedData();
                        tdltdhSelectedData.CompanyName = companyName;
                        tdltdhSelectedData.TDH = highValue;
                        tdltdhSelectedData.TDL = lowValue;
                        tdltdhSelectedData.SelectedDateForTDHAndTDL = marketPriceData.DateTime;
                        break; // Assuming you only want to capture the first instance
                    }
                }

                return tdltdhSelectedData;
            }
            return null;

        }

        public override CTLDCTHDSelectedData GetCTLDCTHDData(DtoRequestModelForCalculate dtoCalculate, decimal currentLow, decimal currentHigh, DateTime currentWorkingDate, string companyName)
        {
            // Generate Session
            _breeze.generateSession(_config.AppSecret, _config.SessionToken);

            var expirydate = DateTime.Parse(dtoCalculate.ExpiryDate);
            var workingDate = DateTime.Parse(dtoCalculate.WorkingDate);

            List<MarketPriceData> marketPrices = null;

            // Filter based on criteria similar to your stored procedure
            if (expirydate == workingDate)
            {
                marketPrices = _breeze.getHistoricalData("day", DateTime.Parse(dtoCalculate.WorkingDate).AddDays(-1).CalculateWorkingDaysBefore(45).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTime.Parse(dtoCalculate.WorkingDate).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    dtoCalculate.Symbole, dtoCalculate.Exchange, dtoCalculate.Type, dtoCalculate.ExpiryDate, "others", "0").ToMarketPriceResponse().Success
                    .OrderByDescending(m => m.DateTime).ToList();
            }
            else if (expirydate > workingDate)
            {
                DateTime lastExpiredDate = DateTimeUtil.GetLastThursdayOfMonth(workingDate);

                // Logic to fetch other necessary dates, similar to SQL queries
                // Example: secondLastExpiredDate, prevExpiryDate, secondPrevExpiryDate

                // Simulating printing out variables
                Console.WriteLine($"@ExpiryDate: {expirydate}");
                Console.WriteLine($"@LastExpiredDate: {lastExpiredDate}");
                // Print other dates as needed

                // Filter market prices based on the retrieved dates
                marketPrices = _breeze.getHistoricalData("day", DateTime.Parse(dtoCalculate.WorkingDate).AddDays(-1).CalculateWorkingDaysBefore(45).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), DateTime.Parse(dtoCalculate.WorkingDate).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    dtoCalculate.Symbole, dtoCalculate.Exchange, dtoCalculate.Type, lastExpiredDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), "others", "0").ToMarketPriceResponse().Success
                    .OrderByDescending(m => m.DateTime).ToList();

            }
            else
            {
                marketPrices = new List<MarketPriceData>();
            }

            // Return top N records based on @NoOfDayOfDataToBeTaken
            var data = marketPrices.Take(45).ToList();

            CTLDCTHDSelectedData ctldcthdSelectedData = new CTLDCTHDSelectedData();

            if (data != null && data.Count > 0)
            {
                decimal highestHigh = currentHigh; // Start with the current highest high
                decimal lowestLow = currentLow;   // Start with the current lowest low

                foreach (var dataPoint in data)
                {
                    decimal high = dataPoint.High;
                    decimal low = dataPoint.Low;

                    if (high > highestHigh)
                        highestHigh = high;

                    if (low < lowestLow)
                        lowestLow = low;
                }

                ctldcthdSelectedData.CTHD = highestHigh;
                ctldcthdSelectedData.CTLD = lowestLow;

                return ctldcthdSelectedData;
            }

            return null;
        }
        #endregion

    }
}
