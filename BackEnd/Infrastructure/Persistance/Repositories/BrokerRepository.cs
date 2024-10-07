using AlgoBhagirath.Common;
using Dapper;
using Domain.Entities.Broker;
using Domain.Repositories;
using System.Data;

namespace Persistance.Repositories
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly RepositoryDbContext _repositoryDbContext;

        public BrokerRepository(RepositoryDbContext repositoryDbContext) => _repositoryDbContext = repositoryDbContext;

        public async Task<IEnumerable<BrokerResponse>> GetAllAsync()
        {
            using var connection = _repositoryDbContext.CreateConnection();
            var customers = await connection.QueryAsync<BrokerResponse>(DatabaseConstants.GetAllBroker, commandType: System.Data.CommandType.StoredProcedure);
            return customers;
        }

        public async Task<Broker> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                BrokerId = brokerId
            });

            using var connection = _repositoryDbContext.CreateConnection();

            var brokerDictionary = new Dictionary<Guid, Broker>();
            var result = await connection.QueryAsync<Broker, BrokerConfiguration, Broker>(DatabaseConstants.GetBrokerById,
            (broker, brokerConfig) =>
            {
                if (!brokerDictionary.TryGetValue(broker.BrokerId, out var currentBroker))
                {
                    currentBroker = broker;
                    currentBroker.BrokerConfigurations = new List<BrokerConfiguration>();
                    brokerDictionary.Add(currentBroker.BrokerId, currentBroker);
                }

                if (brokerConfig != null)
                {
                    brokerConfig.BrokerId = broker.BrokerId;
                    currentBroker.BrokerConfigurations.Add(brokerConfig);
                }

                return currentBroker;
            },
            parameters,
            splitOn: "BrokerConfigurationId",
            commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<bool> InsertOrUpdateAsync(Broker broker)
        {
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                broker.BrokerId,
                broker.Name,
                broker.Description,
                broker.APIDocumentationUrl,
                broker.AuthenticationUrl,
                broker.IsActive,

            });

            // Prepare DataTable for Configurations
            var configTable = new DataTable();
            configTable.Columns.Add("BrokerId", typeof(Guid));
            configTable.Columns.Add("ConfigurationId", typeof(Guid));
            configTable.Columns.Add("ConfigKey", typeof(string));
            configTable.Columns.Add("ConfigValue", typeof(string));
            configTable.Columns.Add("NeedsToUpdateDaily", typeof(bool));
            foreach (var config in broker.BrokerConfigurations)
            {
                if(Guid.Empty == config.BrokerConfigurationId)
                {
                    configTable.Rows.Add(null, null, config.ConfigKey, "", config.NeedsToUpdateDaily);
                }
                else
                {
                    configTable.Rows.Add(config.BrokerId, config.BrokerConfigurationId, config.ConfigKey, "", config.NeedsToUpdateDaily);
                }
            }

            parameters.Add("@Configurations", configTable.AsTableValuedParameter("dbo.ConfigurationsType"));


            using var connection = _repositoryDbContext.CreateConnection();
            var result = await connection.ExecuteAsync(DatabaseConstants.AddOrUpdateBroker, parameters, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task DeleteAsync(Guid brokerId)
        {
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                BrokerId = brokerId
            });

            using var connection = _repositoryDbContext.CreateConnection();
            await connection.ExecuteAsync(DatabaseConstants.DeleteBroker, parameters, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
