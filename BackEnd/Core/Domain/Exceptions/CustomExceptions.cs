namespace Domain.Exceptions
{
    public sealed class StockDataNotFoundException : NotFoundException
    {
        public StockDataNotFoundException(string symbol)
            : base($"The stock with the symbol {symbol} was not found.")
        {
        }
    }

    public sealed class SubscriptionNotFoundException : NotFoundException
    {
        public SubscriptionNotFoundException()
            : base($"Valid Subscription not found.")
        {
        }

    }
}