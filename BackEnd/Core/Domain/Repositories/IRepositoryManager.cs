namespace Domain.Repositories
{
    public interface IRepositoryManager
    {
        IBrokerRepository BrokerRepository { get; }
        IUserBrokerRepository UserBrokerRepository { get; }
        IMasterRepository MasterRepository { get; }
        IMarketDataRepository MarketDataRepository { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
