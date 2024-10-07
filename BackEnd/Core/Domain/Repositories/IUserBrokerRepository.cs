using Domain.Entities.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUserBrokerRepository
    {
        Task<IEnumerable<Broker>> GetBrokerDetailsForUpdateAsync(Guid brokerId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> UpdateBrokerConfiguration(Guid configId, string configValue);
    }
}
