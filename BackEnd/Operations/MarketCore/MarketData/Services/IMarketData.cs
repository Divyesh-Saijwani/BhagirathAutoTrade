using Contracts.Calculation;
using MarketData.Entities;
using MarketDataSync.Models.DTOs;

namespace MarketData.Services
{
    public interface IMarketData
    {
        bool CheckTokenStatusToken(string token);
        Dictionary<string, object> GetCustomerDetails();
        Dictionary<string, object> GetFunds();
        Dictionary<string, object> SetFunds(string transactionType, double amount, string segment);
        Dictionary<string, object> GetDematHoldings();
        MarketPriceResponse GetEquityDataByDate(string exchange, string symbol, DateTime fromDate, DateTime toDate, string interval= "day");
        MarketPriceResponse GetDerivitiveDataByDate(string exchange, string symbol, string product, string expiryDate, string optionType, int strikePrice, DateTime? fromDate, DateTime? toDate, string interval="day");
        ApiLiveOptionDataResponse GetLiveFutureData(string symbol, DateTime expiry);
        ApiLiveOptionDataResponse GetLiveOptionData(string symbol, string optionType, DateTime expiry, string strikePrice);
        IEnumerable<int> GetOptionsStrikePrices(string symbol);
        IEnumerable<DateTime> GetExpiryDates(string symbol, string optionType);
        Task<object> GetCalculatedData(string symbol, string exchange, string type, string instrument, string expiryDate, string date);
    }
}
