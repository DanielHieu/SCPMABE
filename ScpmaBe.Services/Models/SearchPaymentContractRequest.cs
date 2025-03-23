namespace ScpmaBe.Services.Models
{
    public class SearchPaymentContractRequest
    {
        public int PaymentContractId { get; set; }

        public int ContractId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public decimal PricePerMonth { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; }

        public int Status { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
