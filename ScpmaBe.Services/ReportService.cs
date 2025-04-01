using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models.Reports;

namespace ScpmaBe.Services
{
    public class ReportService : IReportService
    {
        private readonly IEntryExitLogRepository _entryExitLogRepository;
        private readonly IParkingLotRepository _parkingLotRepository;
        private readonly IPaymentContractRepository _paymentContractRepository;

        public ReportService(
            IEntryExitLogRepository entryExitLogRepository,
            IParkingLotRepository parkingLotRepository,
            IPaymentContractRepository paymentContractRepository) 
        {
            _entryExitLogRepository = entryExitLogRepository;
            _parkingLotRepository = parkingLotRepository;
            _paymentContractRepository = paymentContractRepository;
        }

        public async Task<List<ParkingLotMonthlyRevenueReport>> GetParkingLotContractMonthlyRevenues(int year)
        {
            var parkingLots = await _parkingLotRepository.GetAll().Select(x => new ParkingLotMonthlyRevenueReport
            {
                ParkingLotName = $"PL{x.ParkingLotId:0#}",
                Month = 1,
                Revenue = 0
            }).ToListAsync();

            var revenues = await _paymentContractRepository
                            .GetAll()
                            .Include(x => x.Contract).Include(x => x.Contract.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                            .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value.Year == year)
                            .GroupBy(x => new { x.Contract.ParkingSpace.Floor.Area.ParkingLotId, x.PaymentDate.Value.Month })
                            .Select(x => new ParkingLotMonthlyRevenueReport
                            {
                                ParkingLotName = $"PL{x.Key.ParkingLotId:0#}",
                                Month = x.Key.Month,
                                Revenue = x.Sum(y => y.PaymentAmount)
                            })
                            .ToListAsync();

            revenues.AddRange(parkingLots);

            return revenues;
        }

        public async Task<List<ParkingLotMonthlyRevenueReport>> GetParkingLotWalkinMonthlyRevenues(int year)
        {
            var parkingLots = await _parkingLotRepository.GetAll().Select(x => new ParkingLotMonthlyRevenueReport
            {
                ParkingLotName = $"PL{x.ParkingLotId:0#}",
                Month = 1,
                Revenue = 0
            }).ToListAsync();

            var revenues = await _entryExitLogRepository
                            .GetAll()
                            .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                            .Where(x => x.ExitTime.HasValue && x.ExitTime.Value.Year == year)
                            .GroupBy(x => new { x.ParkingSpace.Floor.Area.ParkingLotId, x.ExitTime.Value.Month })
                            .Select(x => new ParkingLotMonthlyRevenueReport
                            {
                                ParkingLotName = $"PL{x.Key.ParkingLotId:0#}",
                                Month = x.Key.Month,
                                Revenue = x.Sum(y => y.TotalAmount)
                            })
                            .ToListAsync();

            revenues.AddRange(parkingLots);

            return revenues;
        }

        public async Task<WalkinDailyRevenueResponse> GetWalkinDailyRevenue(DateTime date)
        {
            
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var dailyLogs = await _entryExitLogRepository.GetAll()
                                            .Include(x=>x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                                            .Where(log => log.ExitTime.HasValue &&
                                                          log.ExitTime.Value >= startDate &&
                                                          log.ExitTime.Value < endDate &&
                                                          log.IsPaid && 
                                                          log.TotalAmount > 0)
                                            .Select(x=> new
                                            {
                                                ParkingLotName = $"PL{x.ParkingSpace.Floor.Area.ParkingLotId:0#}",
                                                x.TotalAmount
                                            })
                                            .ToListAsync();

            var revenueByParkingLot = dailyLogs
                                        .GroupBy(log => log.ParkingLotName)
                                        .Select(group => new ParkingLotRevenueResponse
                                        {
                                            ParkingLotName = group.Key,
                                            Revenue = group.Sum(log => log.TotalAmount)
                                        })
                                        .ToList();

            var parkingLots = await _parkingLotRepository
                                        .GetAll()
                                        .Select(x => new ParkingLotRevenueResponse
                                        {
                                            ParkingLotName = $"PL{x.ParkingLotId:0#}",
                                            Revenue = 0
                                        })
                                        .ToListAsync();

            foreach (var item in parkingLots)
            {
                var revenueItem = revenueByParkingLot.FirstOrDefault(x => x.ParkingLotName == item.ParkingLotName);
             
                if (revenueItem != null)
                {
                    item.Revenue = revenueItem.Revenue;
                }
            }

            var totalRevenue = revenueByParkingLot.Sum(item => item.Revenue);

            return await Task.FromResult(new WalkinDailyRevenueResponse
            {
                RevenueDate = date.ToString("dd/MM/yyyy"),
                TotalRevenue = totalRevenue,
                WalkinDailyRevenueDetails = parkingLots
            });
        }
    }
}
