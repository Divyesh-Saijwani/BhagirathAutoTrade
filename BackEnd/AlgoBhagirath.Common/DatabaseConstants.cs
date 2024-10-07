namespace AlgoBhagirath.Common
{
    public class DatabaseConstants
    {
        public const string GetAllBroker = "usp_Broker_GetAll";
        public const string GetBrokerById = "usp_GetBrokerAndConfigurations";
        public const string AddOrUpdateBroker = "usp_AddOrUpdateBrokerAndConfigurations";
        public const string DeleteBroker = "usp_Broker_Delete";

        public const string AddOrUpdateBrokerConfigurations = "usp_AddOrUpdateUserBrokerAndConfigurations";

        public const string GetUserBrokerDetailsForUpdate = "usp_GetUserBrokerDetailsForUpdate";

        public const string GetTodayCalculations = "usp_GetTodayStockCalculations";
    }
}
