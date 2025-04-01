namespace ScpmaBe.Services.Models.Reports
{
    public class WalkinDailyRevenueResponse
    {
        public string RevenueDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ParkingLotRevenueResponse> WalkinDailyRevenueDetails { get; set; }
    }
}
