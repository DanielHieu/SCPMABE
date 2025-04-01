namespace ScpmaBe.Services.Models
{
    public class FeedbackResponse
    {
        public int FeedbackId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public string CreatedAt { get; set; }

        public string Status { get; set; }
        public string ResponsedContent { get; set; }
        public string ResponsedAt { get; set; }
    }
}
