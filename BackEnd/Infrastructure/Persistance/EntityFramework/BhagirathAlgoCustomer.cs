namespace MarketDataSync.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BhagirathAlgoCustomer")]
    public partial class BhagirathAlgoCustomer
    {
        [Key]
        public Guid EquityAlgoCustomerId { get; set; }

        public Guid AlgoCalculationId { get; set; }

        public int? Lots { get; set; }

        public int? Multiplier { get; set; }

        [Required]
        [StringLength(10)]
        public string Direction { get; set; }

        public decimal? StrikePrice { get; set; }

        public Guid BrokerId { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool IsSqaredUp { get; set; }
    }
}
