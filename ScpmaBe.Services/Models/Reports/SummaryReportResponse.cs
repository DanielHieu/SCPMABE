namespace ScpmaBe.Services.Models.Reports
{
    public class SummaryReportResponse
    {
        public int TotalCustomers { get; set; }
        public int TotalStaffs { get; set; }
        public int TotalContracts { get; set; }
        public int TotalWalkins { get; set; }

        public decimal TotalRevenueInYear { get; set; }
        public decimal TotalRevenueInMonth { get; set; }
        public decimal TotalRevenueInWeek { get; set; }
        public decimal TotalRevenueInDay { get; set; }
    }
}
