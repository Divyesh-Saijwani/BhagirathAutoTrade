namespace MarketDataSync.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BrokerConfiguration")]
    public partial class BrokerConfiguration
    {
        public Guid BrokerConfigurationId { get; set; }

        public Guid BrokerId { get; set; }

        [Required]
        [StringLength(50)]
        public string ConfigKey { get; set; }

        public bool NeedsToUpdateDaily { get; set; }

        public virtual Broker Broker { get; set; }
    }
}
