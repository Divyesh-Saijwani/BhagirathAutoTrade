using Contracts.Broker;

namespace Services.Abstractions
{
    public interface IBrokerService
    {
        Task<IEnumerable<BrokerDto>> GetAllAsync();
        Task<BrokerDto> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default);
        Task<bool> InsertOrUpdateAsync(BrokerDto broker);
        Task DeleteAsync(Guid brokerId);
    }
}
