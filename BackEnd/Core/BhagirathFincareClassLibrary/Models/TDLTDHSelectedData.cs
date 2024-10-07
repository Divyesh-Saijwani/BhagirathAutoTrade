using System;

namespace BhagirathFincareClassLibrary.Models
{
    public class TDLTDHSelectedData
    {
        public decimal TDH { get; set; }
        public decimal TDL { get; set; }
        public string CompanyName { get; set; }
        public int FileType { get; set; }
        public DateTime SelectedDateForTDHAndTDL { get; set; }
    }
}
