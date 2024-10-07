namespace MarketDataSync.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserBrokerConfiguration")]
    public partial class UserBrokerConfiguration
    {
        public Guid UserBrokerConfigurationId { get; set; }

        public Guid UserBrokerId { get; set; }

        [Required]
        [StringLength(50)]
        public string ConfigKey { get; set; }

        [Required]
        [StringLength(250)]
        public string ConfigValue { get; set; }

        public bool NeedsToChangeEveryDay { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public virtual UserBroker UserBroker { get; set; }
    }
}
