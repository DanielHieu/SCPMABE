namespace ScpmaBe.Services.Models.Reports
{
    public class MonthlyRevenueByType
    {
        public int Month { get; set; }
        public decimal ContractRevenue { get; set; }
        public decimal WalkinRevenue { get; set; }
    }
}
