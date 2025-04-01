using ScpmaBe.Services.Models.Reports;

namespace ScpmaBe.Services.Interfaces
{
    public interface IReportService
    {
        Task<WalkinDailyRevenueResponse> GetWalkinDailyRevenue(DateTime date);

        Task<List<ParkingLotMonthlyRevenueReport>> GetParkingLotContractMonthlyRevenues(int year);
        Task<List<ParkingLotMonthlyRevenueReport>> GetParkingLotWalkinMonthlyRevenues(int year);
    }
}
