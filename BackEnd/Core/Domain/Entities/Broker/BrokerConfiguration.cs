namespace Domain.Entities.Broker
{
    public class BrokerConfiguration
    {
        public Guid BrokerConfigurationId { get; set; }
        public Guid BrokerId { get; set; }
        public string ConfigKey { get; set; }
        public bool NeedsToUpdateDaily { get; set; }
    }
}
