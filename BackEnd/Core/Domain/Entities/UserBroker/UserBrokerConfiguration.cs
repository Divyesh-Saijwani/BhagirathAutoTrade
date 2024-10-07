namespace Domain.Entities.UserBroker
{
    using System;

    public class UserBrokerConfiguration
    {
        public Guid UserBrokerConfigurationId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public bool NeedsToChangeEveryDay { get; set; }
        public DateTime LastModifiedDate { get; set; }

    }
}
