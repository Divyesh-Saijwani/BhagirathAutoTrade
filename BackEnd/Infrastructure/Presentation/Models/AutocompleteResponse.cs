namespace BhagirathAutoTrade.Server.Models
{
    public class AutocompleteResponse
    {
        public IEnumerable<string> Data { get; set; }
        public string Message { get; set; }
        public int Success { get; set; }
    }
}
