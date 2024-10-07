namespace Services.Abstractions
{
    public interface IServiceManager
    {
        IAccountService AccountService { get; }
        IMasterService MasterDataService { get; }
        IBrokerService BrokerService { get; }
        IMarketDataService MarketDataService { get; }
        IUserBrokerService UserBrokerService { get; }
    }
}
