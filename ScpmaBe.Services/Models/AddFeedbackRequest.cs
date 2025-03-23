namespace ScpmaBe.Services.Models
{
    public class AddFeedbackRequest
    {
        public int CustomerId { get; set; }
        public string Message { get; set; }
        public DateOnly DaySubmitted { get; set; }
    }
}
