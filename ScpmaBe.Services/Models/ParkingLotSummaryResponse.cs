namespace ScpmaBe.Services.Models
{
    public class ParkingLotSummaryResponse
    {
        public int TotalAreas { get; set; }
        public int TotalParkingSpaces { get; set; }
        public int TotalAvailableParkingSpaces { get; set; }
        public decimal TotalRevenues { get; set; }
    }
}
