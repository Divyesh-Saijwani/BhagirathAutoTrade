namespace MarketDataSync.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserBroker")]
    public partial class UserBroker
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserBroker()
        {
            UserBrokerConfigurations = new HashSet<UserBrokerConfiguration>();
        }

        public Guid UserBrokerId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string BrokerId { get; set; }

        public bool IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserBrokerConfiguration> UserBrokerConfigurations { get; set; }
    }
}
