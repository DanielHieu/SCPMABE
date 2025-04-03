namespace ScpmaBe.Services.Models
{
    public class PaymentContractResponse
    {
        public int PaymentContractId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }   
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}
