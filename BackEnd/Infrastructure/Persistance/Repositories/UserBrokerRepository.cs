using AlgoBhagirath.Common;
using Dapper;
using Domain.Entities.Broker;
using Domain.Entities.UserBroker;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Persistance.Repositories
{
    public class UserBrokerRepository : IUserBrokerRepository
    {
        private readonly RepositoryDbContext _repositoryDbContext;

        public UserBrokerRepository(RepositoryDbContext repositoryDbContext) => _repositoryDbContext = repositoryDbContext;
        public async Task<IEnumerable<Broker>> GetBrokerDetailsForUpdateAsync(Guid brokerId, Guid userId, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("BrokerId", brokerId.ToString());
            parameters.Add("UserId", userId.ToString());

            using var connection = _repositoryDbContext.CreateConnection();

            var brokerDictionary = new Dictionary<Guid, Broker>();
            var userBrokerConfigurations = new List<UserBrokerConfiguration>();

            var result = await connection.QueryAsync<Broker, UserBroker, UserBrokerConfiguration, Broker>(
                DatabaseConstants.GetUserBrokerDetailsForUpdate,
                (broker, userBroker, userBrokerConfig) =>
                {
                    if (!brokerDictionary.TryGetValue(broker.BrokerId, out var currentBroker))
                    {
                        currentBroker = broker;
                        currentBroker.BrokerConfigurations = new List<BrokerConfiguration>();
                        brokerDictionary.Add(currentBroker.BrokerId, currentBroker);
                    }

                    if (userBrokerConfig != null)
                    {
                        userBrokerConfigurations.Add(userBrokerConfig);
                    }

                    return currentBroker;
                },
                parameters,
                splitOn: "UserBrokerId,UserBrokerConfigurationId",
                commandType: CommandType.StoredProcedure);

            // Process the user broker configurations for token replacement
            foreach (var broker in brokerDictionary.Values)
            {
                foreach (var userBrokerConfig in userBrokerConfigurations)
                {
                    broker.AuthenticationUrl = ReplaceTokens(broker.AuthenticationUrl, userBrokerConfig.ConfigKey, HttpUtility.UrlEncode(userBrokerConfig.ConfigValue));
                    if (userBrokerConfig.NeedsToChangeEveryDay) {
                        broker.BrokerConfigurations.Add( new BrokerConfiguration { ConfigKey = userBrokerConfig.ConfigKey, NeedsToUpdateDaily= userBrokerConfig.NeedsToChangeEveryDay });
                    }
                }
            }

            return brokerDictionary.Values.Distinct().ToList();
        }

        public async Task<bool> UpdateBrokerConfiguration(Guid configId, string configValue)
        {
            try
            {
                const string query = $"UPDATE [dbo].[UserBrokerConfiguration] SET [ConfigValue] = @ConfigValue, [LastModifiedDate]=GETDATE() WHERE [UserBrokerConfigurationId] = @ConfigId";

                using (var connection = _repositoryDbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { ConfigId = configId, ConfigValue = configValue });
                    return true; // Return true if the operation affected one or more rows
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string ReplaceTokens(string url, string token, string value)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(token) || value == null)
                return url;

            return url.Replace($"[{token}]", value);
        }



    }
}
