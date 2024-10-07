namespace Contracts.Broker
{
    public class BrokerDto
    {
        public Guid? BrokerId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? APIDocumentationUrl { get; set; }
        public string? AuthenticationUrl { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<BrokerConfigurationDto> BrokerConfigurations { get; set; }
    }

    public class BrokerConfigurationDto
    {
        public Guid? BrokerConfigurationId { get; set; }
        public Guid? BrokerId { get; set; }
        public string ConfigKey { get; set; }
        public bool NeedsToUpdateDaily { get; set; }
    }
}
