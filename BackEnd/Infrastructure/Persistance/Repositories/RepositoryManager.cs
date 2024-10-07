using Domain.Repositories;

namespace Persistance.Repositories
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IMasterRepository> _lazyMasterRepository;
        private readonly Lazy<IBrokerRepository> _lazyBrokerRepository;
        private readonly Lazy<IUserBrokerRepository> _lazyUserBrokerRepository;
        private readonly Lazy<IMarketDataRepository> _lazyMarketDataRepository;
        private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;

        public RepositoryManager(RepositoryDbContext repositoryDbContext)
        {
            _lazyMasterRepository = new Lazy<IMasterRepository>(() => new MasterRepository(repositoryDbContext));
            _lazyBrokerRepository = new Lazy<IBrokerRepository>(() => new BrokerRepository(repositoryDbContext));
            _lazyUserBrokerRepository=new Lazy<IUserBrokerRepository>(() => new UserBrokerRepository(repositoryDbContext));
            _lazyMarketDataRepository = new Lazy<IMarketDataRepository>(() => new MarketDataRepository(repositoryDbContext));
            _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(repositoryDbContext));
        }
        public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
        public IMasterRepository MasterRepository => _lazyMasterRepository.Value;
        public IBrokerRepository BrokerRepository => _lazyBrokerRepository.Value;
        public IUserBrokerRepository UserBrokerRepository => _lazyUserBrokerRepository.Value;
        public IMarketDataRepository MarketDataRepository => _lazyMarketDataRepository.Value;
    }
}
