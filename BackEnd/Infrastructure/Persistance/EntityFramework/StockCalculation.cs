namespace MarketDataSync.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StockCalculation")]
    public partial class StockCalculation
    {
        [Key]
        public Guid AlgoCalculationId { get; set; }

        [Required]
        [StringLength(50)]
        public string Symbol { get; set; }

        [Required]
        [StringLength(10)]
        public string Exchange { get; set; }

        [StringLength(10)]
        public string Type { get; set; }

        [StringLength(10)]
        public string Instrument { get; set; }

        [StringLength(5)]
        public string OptionType { get; set; }

        public decimal? StrikePrice { get; set; }

        public DateTime? Expiry { get; set; }

        public decimal LowPoint { get; set; }

        public decimal AveragePoint { get; set; }

        public decimal MaxPoint { get; set; }

        [Required]
        [StringLength(10)]
        public string Direction { get; set; }

        public DateTime EntryTime { get; set; }

        public decimal StopLoss { get; set; }

        public decimal Target { get; set; }

        public DateTime CalculationDateTime { get; set; }

        public bool IsActive { get; set; }
    }
}
