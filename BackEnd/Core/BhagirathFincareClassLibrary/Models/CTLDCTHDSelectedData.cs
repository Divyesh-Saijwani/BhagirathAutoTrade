using System;

namespace BhagirathFincareClassLibrary.Models
{
    public class CTLDCTHDSelectedData
    {
        public decimal CTHD { get; set; }
        public decimal CTLD { get; set; }
        public string CompanyName { get; set; }
        public int FileType { get; set; }
        public DateTime SelectedDateForCTHDAndCTLD { get; set; }
    }
}
