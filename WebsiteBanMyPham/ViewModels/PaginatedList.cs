namespace WebsiteBanMyPham.ViewModels
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedList()
        {
        }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = Math.Max(1, pageIndex);
            PageSize = pageSize > 0 ? pageSize : 12;
            TotalItems = count;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            Items = items;
        }
    }
}

