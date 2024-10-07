using Domain.Entities.Broker;

namespace Domain.Repositories
{
    public interface IBrokerRepository
    {
        Task<IEnumerable<BrokerResponse>> GetAllAsync();
        Task<Broker> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default);
        Task<bool> InsertOrUpdateAsync(Broker broker);
        Task DeleteAsync(Guid brokerId);
    }
}
