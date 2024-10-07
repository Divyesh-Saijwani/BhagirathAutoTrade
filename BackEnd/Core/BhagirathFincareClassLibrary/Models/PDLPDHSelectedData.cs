using System;

namespace BhagirathFincareClassLibrary.Models
{
    public class PDLPDHSelectedData
    {
        public decimal PDH { get; set; }
        public decimal PDL { get; set; }
        public string CompanyName { get; set; }
        public int FileType { get; set; }
        public DateTime SelectedDateForPDHAndPDL { get; set; }
    }
}
