using Contracts.Broker;
using Domain.Entities.Broker;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserBrokerService : IUserBrokerService
    {

        private readonly IRepositoryManager _repositoryManager;
        public UserBrokerService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<BrokerDto>> GetBrokerDetailsForUpdateAsync(Guid brokerId, Guid userId, CancellationToken cancellationToken = default)
        {
            var result= await _repositoryManager.UserBrokerRepository.GetBrokerDetailsForUpdateAsync(brokerId, userId, cancellationToken);
            return result.Adapt<IEnumerable<BrokerDto>>();
        }

        public async Task<bool> UpdateBrokerConfiguration(Guid configId, string configValue)
        {
            var result = await _repositoryManager.UserBrokerRepository.UpdateBrokerConfiguration(configId, configValue);
            return result;
        }
    }
}
