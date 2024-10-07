using Domain.Repositories;
using Identity.Services;
using Services.Abstractions;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {

        private readonly Lazy<IMasterService> _lazyMasterService;
        private readonly Lazy<IBrokerService> _lazyBrokerService;
        private readonly Lazy<IUserBrokerService> _lazyUserBrokerService;
        private readonly Lazy<IAccountService> _lazyAccountService;
        private readonly Lazy<IMarketDataService> _lazyMarketDataService;


        public ServiceManager(IRepositoryManager repositoryManager,IUserService userService)
        {
            _lazyMasterService = new Lazy<IMasterService>(() => new MasterDataService(repositoryManager));
            _lazyBrokerService = new Lazy<IBrokerService>(() => new BrokerService(repositoryManager));
            _lazyUserBrokerService = new Lazy<IUserBrokerService>(() => new UserBrokerService(repositoryManager));
            _lazyAccountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager, userService));
            _lazyMarketDataService=new Lazy<IMarketDataService>(()=>new MarketDataService(repositoryManager,userService));
        }

        public IMasterService MasterDataService => _lazyMasterService.Value;
        public IBrokerService BrokerService => _lazyBrokerService.Value;
        public IAccountService AccountService => _lazyAccountService.Value;
        public IUserBrokerService UserBrokerService => _lazyUserBrokerService.Value;
        public IMarketDataService MarketDataService => _lazyMarketDataService.Value;
    }
}
