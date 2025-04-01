namespace ScpmaBe.Services.Models
{
    public class CalculateFeeResponse
    {
        public int Id { get; set; }
        public int ParkingSpaceId { get; set; }

        public string LicensePlate { get; set; }
        public string EntranceImage { get; set; }
        
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }

        public ContractResponse Contract { get; set; }
        public decimal Fee { get; set; }
        public int RemainingHour { get; set; }

        public string RentalType { get; set; }
        public string CalculationNotes { get; set; }
    }
}
