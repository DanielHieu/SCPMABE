namespace ScpmaBe.Services.Models
{
    public class SearchFeedbackRequest
    {
        public string Keyword { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Status { get; set; }
    }
}
