namespace MarketDataSync.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MarketPrice")]
    public partial class MarketPrice
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Symbol { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string StrikePrice { get; set; }

        [StringLength(50)]
        public string OptionType { get; set; }

        [StringLength(20)]
        public string Open { get; set; }

        [StringLength(20)]
        public string High { get; set; }

        [StringLength(20)]
        public string Low { get; set; }

        [StringLength(20)]
        public string Close { get; set; }

        public DateTime? UploadDate { get; set; }

        public int FileType { get; set; }

        [StringLength(100)]
        public string ScriptCode { get; set; }

        [StringLength(10)]
        public string EMA13 { get; set; }

        [StringLength(10)]
        public string EMA34 { get; set; }

        [StringLength(50)]
        public string TotalTradeValue { get; set; }
    }
}
