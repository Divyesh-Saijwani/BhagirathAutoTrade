using Contracts.Broker;
using Domain.Repositories;
using Services.Abstractions;
using Mapster;
using Domain.Entities.Broker;

namespace Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IRepositoryManager _repositoryManager;
        public BrokerService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<BrokerDto>> GetAllAsync()
        {
            var brokers= await this._repositoryManager.BrokerRepository.GetAllAsync();
            return brokers.Adapt<IEnumerable<BrokerDto>>();
        }

        public async Task<BrokerDto> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default)
        {
            var brokers = await this._repositoryManager.BrokerRepository.GetByIdAsync(brokerId,cancellationToken);
            return brokers.Adapt<BrokerDto>();
        }

        public async Task<bool> InsertOrUpdateAsync(BrokerDto broker)
        {
            var brokerObj=broker.Adapt<Broker>();
            var result = await this._repositoryManager.BrokerRepository.InsertOrUpdateAsync(brokerObj);
            return result;
        }

        public async Task DeleteAsync(Guid brokerId)
        {
            await this._repositoryManager.BrokerRepository.DeleteAsync(brokerId);
        }

        public async Task GetUserBrokerAction(Guid brokerId)
        {

        }
    }
}
