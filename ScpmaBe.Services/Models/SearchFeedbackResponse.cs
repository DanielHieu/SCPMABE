namespace ScpmaBe.Services.Models
{
    public class SearchFeedbackResponse
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<FeedbackResponse> Items { get; set; } = new List<FeedbackResponse>();
    }
}
