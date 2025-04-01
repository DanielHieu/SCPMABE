namespace ScpmaBe.Services.Models
{
    public class ParkingLotStatsResponse
    {
        public int TotalSpaces {get; set; }
        public int OccupiedSpaces { get; set; }
        public int RevenueToday { get; set; }
        public decimal PercentageOccupied { get; set; }
        public int VisitsToday { get; set; }
        public string UpdatedAt { get; set; }
    }
}
