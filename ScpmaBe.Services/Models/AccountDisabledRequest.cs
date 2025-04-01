namespace ScpmaBe.Services.Models
{
    public class AccountDisabledRequest
    {
        public int CustomerId { get; set; }
        public string Reason { get; set; }
    }
}
