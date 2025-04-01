using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models.Reports;

namespace ScpmaBe.Services
{
    public class ReportService : IReportService
    {
        private readonly IEntryExitLogRepository _entryExitLogRepository;
        private readonly IParkingLotRepository _parkingLotRepository;
        private readonly IPaymentContractRepository _paymentContractRepository;

        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IContractRepository _contractRepository;

        public ReportService(
            IEntryExitLogRepository entryExitLogRepository,
            IParkingLotRepository parkingLotRepository,
            IPaymentContractRepository paymentContractRepository,
            ICustomerRepository customerRepository,
            IStaffRepository staffRepository,
            IContractRepository contractRepository)
        {
            _entryExitLogRepository = entryExitLogRepository;
            _parkingLotRepository = parkingLotRepository;
            _paymentContractRepository = paymentContractRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _contractRepository = contractRepository;
        }
        public async Task<List<MonthlyRevenueByType>> GetMonthlyRevenueByTypes(int year)
        {
            var result = new List<MonthlyRevenueByType>();

            // Initialize result with all months
            for (int month = 1; month <= 12; month++)
            {
                result.Add(new MonthlyRevenueByType
                {
                    Month = month,
                    ContractRevenue = 0,
                    WalkinRevenue = 0
                });
            }

            // Get contract revenue by month
            var contractRevenues = await _paymentContractRepository
                .GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value.Year == year)
                .GroupBy(x => x.PaymentDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.PaymentAmount)
                })
                .ToListAsync();

            // Get walkin revenue by month
            var walkinRevenues = await _entryExitLogRepository
                .GetAll()
                .Where(x => x.ExitTime.HasValue &&
                       x.ExitTime.Value.Year == year &&
                       x.TotalAmount > 0)
                .GroupBy(x => x.ExitTime.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            // Update result with actual revenues
            foreach (var contractRevenue in contractRevenues)
            {
                var monthData = result.FirstOrDefault(x => x.Month == contractRevenue.Month);
                if (monthData != null)
                {
                    monthData.ContractRevenue = contractRevenue.Revenue;
                }
            }

            foreach (var walkinRevenue in walkinRevenues)
            {
                var monthData = result.FirstOrDefault(x => x.Month == walkinRevenue.Month);
                if (monthData != null)
                {
                    monthData.WalkinRevenue = walkinRevenue.Revenue;
                }
            }

            return result;
        }

        public async Task<List<ParkingLotMonthlyRevenueReport>> GetParkingLotContractMonthlyRevenues(int year)
        {
            var parkingLots = await _parkingLotRepository.GetAll().Select(x => new ParkingLotMonthlyRevenueReport
            {
                ParkingLotId = x.ParkingLotId,
                ParkingLotName = x.ParkingLotName,
                Month = 1,
                Revenue = 0
            }).ToListAsync();

            var revenues = await _paymentContractRepository
                                    .GetAll()
                                    .Include(x => x.Contract).Include(x => x.Contract.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                    .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value.Year == year)
                                    .GroupBy(x =>
                                        new
                                        {
                                            x.Contract.ParkingSpace.Floor.Area.ParkingLotId,
                                            ParkingLotMonth = new { x.Contract.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName, x.PaymentDate.Value.Month }
                                        }
                                    )
                                    .Select(x => new ParkingLotMonthlyRevenueReport
                                    {
                                        ParkingLotId = x.Key.ParkingLotId,
                                        ParkingLotName = x.Key.ParkingLotMonth.ParkingLotName,
                                        Month = x.Key.ParkingLotMonth.Month,
                                        Revenue = x.Sum(y => y.PaymentAmount)
                                    })
                                    .ToListAsync();

            revenues.AddRange(parkingLots);

            return revenues;
        }

        public async Task<List<ParkingLotRevenueResponse>> GetParkingLotRevenueInYearReports()
        {
            var now = DateTime.Now.ToVNTime();
            var startOfYear = new DateTime(now.Year, 1, 1);

            // Get all parking lots
            var parkingLots = await _parkingLotRepository.GetAll().ToListAsync();

            var result = new List<ParkingLotRevenueResponse>();

            foreach (var parkingLot in parkingLots)
            {
                // Calculate contract revenue for this parking lot
                var contractRevenue = await _paymentContractRepository
                    .GetAll()
                    .Include(x => x.Contract)
                    .Include(x => x.Contract.ParkingSpace)
                    .ThenInclude(x => x.Floor)
                    .ThenInclude(x => x.Area)
                    .Where(x => x.PaymentDate >= startOfYear &&
                                x.PaymentDate <= now &&
                                x.Contract.ParkingSpace.Floor.Area.ParkingLotId == parkingLot.ParkingLotId)
                    .SumAsync(x => x.PaymentAmount);

                // Calculate walk-in revenue for this parking lot
                var walkinRevenue = await _entryExitLogRepository
                    .GetAll()
                    .Include(x => x.ParkingSpace)
                    .ThenInclude(x => x.Floor)
                    .ThenInclude(x => x.Area)
                    .Where(x => x.ExitTime.HasValue &&
                           x.ExitTime.Value >= startOfYear &&
                           x.ExitTime.Value <= now &&
                           x.ParkingSpace.Floor.Area.ParkingLotId == parkingLot.ParkingLotId)
                    .SumAsync(x => x.TotalAmount);

                // Add to result
                result.Add(new ParkingLotRevenueResponse
                {
                    ParkingLotName = parkingLot.ParkingLotName,
                    Revenue = contractRevenue + walkinRevenue
                });
            }

            return result;
        }

        public async Task<List<ParkingLotMonthlyRevenueReport>> GetParkingLotWalkinMonthlyRevenues(int year)
        {
            var parkingLots = await _parkingLotRepository.GetAll().Select(x => new ParkingLotMonthlyRevenueReport
            {
                ParkingLotName = x.ParkingLotName,
                Month = 1,
                Revenue = 0
            }).ToListAsync();

            var revenues = await _entryExitLogRepository
                            .GetAll()
                            .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                            .Where(x => x.ExitTime.HasValue && x.ExitTime.Value.Year == year)
                            .GroupBy(x =>
                                new
                                {
                                    x.ParkingSpace.Floor.Area.ParkingLotId,
                                    ParkingLotMonth = new { x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName, x.ExitTime.Value.Month }
                                }
                            )
                            .Select(x => new ParkingLotMonthlyRevenueReport
                            {
                                ParkingLotId = x.Key.ParkingLotId,
                                ParkingLotName = x.Key.ParkingLotMonth.ParkingLotName,
                                Month = x.Key.ParkingLotMonth.Month,
                                Revenue = x.Sum(y => y.TotalAmount)
                            })
                            .ToListAsync();

            revenues.AddRange(parkingLots);

            return revenues;
        }

        public async Task<SummaryReportResponse> GetSummaryReport()
        {
            var now = DateTime.Now.ToVNTime();

            var startOfDay = now.Date;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);

            // Get total customers
            var totalCustomers = await _customerRepository.GetAll().CountAsync();

            // Get total staffs
            var totalStaffs = await _staffRepository.GetAll().CountAsync();

            // Get total contracts
            var totalContracts = await _contractRepository.GetAll().CountAsync();

            // Get total walkins (entry-exit logs that are paid)
            var totalWalkins = await _entryExitLogRepository.GetAll()
                .Where(x => x.IsPaid && x.TotalAmount > 0)
                .CountAsync();

            // Get revenue from contracts
            var contractRevenueInDay = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfDay)
                .SumAsync(x => x.PaymentAmount);

            var contractRevenueInWeek = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfWeek)
                .SumAsync(x => x.PaymentAmount);

            var contractRevenueInMonth = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfMonth)
                .SumAsync(x => x.PaymentAmount);

            var contractRevenueInYear = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfYear)
                .SumAsync(x => x.PaymentAmount);

            // Get revenue from walkins
            var walkinRevenueInDay = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfDay && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            var walkinRevenueInWeek = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfWeek && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            var walkinRevenueInMonth = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfMonth && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            var walkinRevenueInYear = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfYear && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            return new SummaryReportResponse
            {
                TotalCustomers = totalCustomers,
                TotalStaffs = totalStaffs,
                TotalContracts = totalContracts,
                TotalWalkins = totalWalkins,
                TotalRevenueInDay = contractRevenueInDay + walkinRevenueInDay,
                TotalRevenueInWeek = contractRevenueInWeek + walkinRevenueInWeek,
                TotalRevenueInMonth = contractRevenueInMonth + walkinRevenueInMonth,
                TotalRevenueInYear = contractRevenueInYear + walkinRevenueInYear
            };
        }

        public async Task<SummaryRevenueReportResponse> GetSummaryRevenueReport()
        {
            var now = DateTime.Now.ToVNTime();

            var startOfDay = now.Date;
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);

            // Get revenue from contracts
            var contractRevenueInDay = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfDay)
                .SumAsync(x => x.PaymentAmount);

            var contractRevenueInWeek = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfWeek)
                .SumAsync(x => x.PaymentAmount);

            var contractRevenueInMonth = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfMonth)
                .SumAsync(x => x.PaymentAmount);

            var contractRevenueInYear = await _paymentContractRepository.GetAll()
                .Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= startOfYear)
                .SumAsync(x => x.PaymentAmount);

            // Get revenue from walkins
            var walkinRevenueInDay = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfDay && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            var walkinRevenueInWeek = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfWeek && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            var walkinRevenueInMonth = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfMonth && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            var walkinRevenueInYear = await _entryExitLogRepository.GetAll()
                .Where(x => x.ExitTime.HasValue && x.ExitTime.Value >= startOfYear && x.IsPaid)
                .SumAsync(x => x.TotalAmount);

            return new SummaryRevenueReportResponse
            {
                TotalRevenueInDay = contractRevenueInDay + walkinRevenueInDay,
                ContractRevenueInDay = contractRevenueInDay,
                WalkinRevenueInDay = walkinRevenueInDay,

                TotalRevenueInWeek = contractRevenueInWeek + walkinRevenueInWeek,
                ContractRevenueInWeek = contractRevenueInWeek,
                WalkinRevenueInWeek = walkinRevenueInWeek,

                TotalRevenueInMonth = contractRevenueInMonth + walkinRevenueInMonth,
                ContractRevenueInMonth = contractRevenueInMonth,
                WalkinRevenueInMonth = walkinRevenueInMonth,

                TotalRevenueInYear = contractRevenueInYear + walkinRevenueInYear,
                ContractRevenueInYear = contractRevenueInYear,
                WalkinRevenueInYear = walkinRevenueInYear
            };
        }

        public async Task<WalkinDailyRevenueResponse> GetWalkinDailyRevenue(DateTime date)
        {

            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var dailyLogs = await _entryExitLogRepository.GetAll()
                                            .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                            .Where(log => log.ExitTime.HasValue &&
                                                          log.ExitTime.Value >= startDate &&
                                                          log.ExitTime.Value < endDate &&
                                                          log.IsPaid &&
                                                          log.TotalAmount > 0)
                                            .Select(x => new
                                            {
                                                x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
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
                                            ParkingLotName = x.ParkingLotName,
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
