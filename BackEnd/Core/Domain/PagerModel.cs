namespace Domain
{
    public class PagerModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public string? SortColumn { get; set; }

    }
}