namespace Domain.Entities.Broker
{
    public class Broker
    {
        public Guid BrokerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string APIDocumentationUrl { get; set; }
        public string AuthenticationUrl { get; set; }
        public bool IsActive { get; set; }

        public List<BrokerConfiguration> BrokerConfigurations { get; set; }
    }

    public class BrokerResponse : Broker
    {
        public int TotalRows { get; set; }
    }

}
