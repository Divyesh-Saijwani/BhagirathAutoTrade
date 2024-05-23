using BhagirathAutoTrade.Server.Models;

namespace BhagirathAutoTrade.Server.Services.Interfaces
{
    public interface IEquityService
    {
        Task<IEnumerable<string>> AutoCompleteCompanyForEquityAsync(string exchange, string type, string instrument, string optionType);
        Task<EquityData> GetCalculateDataForEQAsync(EquityRequestModel request);
        Task<IEnumerable<string>> GetExpiryDateAsync(string symbol, string optionType);
        Task<string> GetOpenDataAsync(string workingDate, string expiryDate, string exchange, string instrument, string optionType, string type, string strickPrice, string symbol);
        Task<string> GetCloseDataAsync(string workingDate, string expiryDate, string exchange, string instrument, string optionType, string type, string strickPrice, string symbol);
        Task<List<string>> GetStrikePriceAsync(string exchange, string type, string symbol, string expiryDate);
    }
}
