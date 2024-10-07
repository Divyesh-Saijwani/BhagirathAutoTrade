namespace BhagirathAutoTrade.Server.Models
{
    public class APIResponseModel<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int Success { get; set; }
    }
}
