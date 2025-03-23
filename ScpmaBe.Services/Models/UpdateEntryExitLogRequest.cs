namespace ScpmaBe.Services.Models
{
    public class UpdateEntryExitLogRequest
    {
        public int EntryExitLogId { get; set; }
        
        public DateTime ExitTime { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
