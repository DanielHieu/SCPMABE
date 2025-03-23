namespace ScpmaBe.Services.Models
{
    public  class SearchContractRequest
    {
        public string Keyword { get; set; }
        public string LicencePlate { get; set; }
        public int? Status { get; set; }
    }
}
