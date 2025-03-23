namespace ScpmaBe.Services.Models
{
    public  class AddEntryExitLogRequest
    {
        public int? ParkingSpaceId { get; set; }

        public string LicensePlate { get; set; }

        public decimal PricePerHour { get; set; }

        public decimal PricePerDay { get; set; }

        public decimal PricePerMonth { get; set; }

        public int RentalType { get; set; }
    }
}
