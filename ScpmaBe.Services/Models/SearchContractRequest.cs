namespace ScpmaBe.Services.Models
{
    public  class SearchContractRequest
    {
        public string? Keyword { get; set; }
        public string? LicensePlate { get; set; }
        public int? Status { get; set; }
        public int? ParkingLotId { get; set; }
    }
}
