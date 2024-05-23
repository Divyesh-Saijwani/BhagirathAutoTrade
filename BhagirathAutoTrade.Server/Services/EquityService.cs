using BhagirathAutoTrade.Server.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using JsonSerializer = System.Text.Json.JsonSerializer;
using BhagirathAutoTrade.Server.Services.Interfaces;
using System.Diagnostics.Metrics;

namespace BhagirathAutoTrade.Server.Services
{
    public class EquityService : IEquityService
    {
        private readonly HttpClient _httpClient;

        public EquityService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://api.bhagirathfincare.in/api/Equity/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<string>> AutoCompleteCompanyForEquityAsync(string exchange, string type, string instrument, string optionType)
        {
            try
            {
                var query = $"AutoCompleteCompanyForEquity?exchange={exchange}&type={type}&instrument={instrument}&optionType={optionType}";

                // Sample data for POST request
                var response = await _httpClient.PostAsync(query, null);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<AutocompleteResponse>(content);

                return result.Data;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetOpenDataAsync(string workingDate, string expiryDate, string exchange, string instrument, string optionType, string type, string strickPrice, string symbol)
        {
            try
            {
                var url = $"GetOpenData?workingDate={workingDate}&expiryDate={expiryDate}&exchange={exchange}&instrument={instrument}&optionType={optionType}&type={type}&strickPrice={strickPrice}&symbole={symbol}";

                var response = await _httpClient.PostAsync(url, null);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<APIResponseModel<string>>(responseData);

                return result.Data;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetCloseDataAsync(string workingDate, string expiryDate, string exchange, string instrument, string optionType, string type, string strickPrice, string symbol)
        {
            try
            {
                var url = $"GetCloseData?workingDate={workingDate}&expiryDate={expiryDate}&exchange={exchange}&instrument={instrument}&optionType={optionType}&type={type}&strickPrice={strickPrice}&symbole={symbol}";

                var response = await _httpClient.PostAsync(url, null);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<APIResponseModel<string>>(responseData);

                return result.Data;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<EquityData> GetCalculateDataForEQAsync(EquityRequestModel request)
        {
            try
            {
                var url = $"getCalculateDataForEQ?exchange={request.Exchange}&type={request.Type}&symbol={request.Symbole}&workingdate={request.WorkingDate}&expirydate={request.ExpiryDate}&close={request.Close}&instrument={request.Instrument}&optionType={request.OptionType}";
                var myContent = JsonConvert.SerializeObject("");
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.PostAsync(url, byteContent);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<EquityApiResponse<EquityData>>(jsonResponse);

                return responseData.Data;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetExpiryDateAsync(string symbol, string optionType)
        {
            try
            {
                var query = $"AutoCompleteexpiry";
                var requestModel = new
                {
                    Symbole = symbol,
                    OptionType = optionType
                };
                var myContent = JsonConvert.SerializeObject(requestModel);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // Sample data for POST request
                var response = await _httpClient.PostAsync(query, byteContent);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<AutocompleteResponse>(content);

                return result.Data;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetStrikePriceAsync(string exchange, string type, string symbol, string expiryDate)
        {
            try
            {
                var url = $"GetStrikePrice?exchange={exchange}&type={type}&symbol={symbol}&expireDate={expiryDate}";

                var response = await _httpClient.PostAsync(url, null);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<APIResponseModel<List<string>>>(responseData);

                return result.Data;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

    }
}
