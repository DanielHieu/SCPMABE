namespace ScpmaBe.Services.Models
{
    public class SearchResultResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }

        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;

        public SearchResultResponse(int totalCount, List<T> items)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }
}
