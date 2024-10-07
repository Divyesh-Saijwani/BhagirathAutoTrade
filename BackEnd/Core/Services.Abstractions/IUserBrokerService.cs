using Contracts.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IUserBrokerService
    {
        Task<IEnumerable<BrokerDto>> GetBrokerDetailsForUpdateAsync(Guid brokerId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> UpdateBrokerConfiguration(Guid configId, string apisession);
    }
}
