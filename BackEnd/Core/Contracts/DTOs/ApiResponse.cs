using Newtonsoft.Json;

namespace MarketDataSync.Models.DTOs
{
    public class ApiResponse<T>
    {
        [JsonProperty("Error")]
        public object Error { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("Success")]
        public List<T> Success { get; set; }
    }
}