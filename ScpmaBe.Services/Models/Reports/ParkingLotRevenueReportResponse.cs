namespace ScpmaBe.Services.Models.Reports
{
    public class ParkingLotRevenueReportResponse
    {
        public string ParkingLotName { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueInDay { get; set; }
        public decimal RevenueInWeek { get; set; }
        public decimal RevenueInMonth { get; set; }
        public decimal RevenueInYear { get; set; }
    }
}
