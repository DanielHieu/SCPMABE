namespace ScpmaBe.Services.Models
{
    public class PaymentContractResponse
    {
        public int PaymentContractId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }   
        public decimal PricePerMonth { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDate { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }

        public int ContractId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string StartDateString
        {
            get
            {
                if (StartDate == default)
                    return string.Empty;
                return StartDate.ToString("dd/MM/yyyy");
            }
        }

        public string EndDateString
        {
            get
            {
                if (EndDate == default)
                    return string.Empty;
                return EndDate.ToString("dd/MM/yyyy");
            }
        }
    }
}
