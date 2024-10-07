namespace Domain.Entities.UserBroker
{
    public class UserBroker
    {
        public Guid UserBrokerId { get; set; }
        public Guid UserId { get; set; }
        public Guid BrokerId { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<UserBrokerConfiguration> UserBrokerConfigurations { get; set; }
    }
}
