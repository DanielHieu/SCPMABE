using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public class ParkingLotService : IParkingLotService
    {
        private readonly IParkingLotRepository _parkinglotRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IParkingSpaceRepository _parkingSpaceRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IPaymentContractRepository _paymentContractRepository;
        private readonly IEntryExitLogRepository _entryExitLogRepository;

        public ParkingLotService(IParkingLotRepository parkinglotRepository,
            IParkingSpaceRepository parkingSpaceRepository,
            IFloorRepository floorRepository,
            IAreaRepository areaRepository,
            IContractRepository contractRepository,
            IPaymentContractRepository paymentContractRepository,
            IEntryExitLogRepository entryExitLogRepository)
        {

            _parkinglotRepository = parkinglotRepository;
            _parkingSpaceRepository = parkingSpaceRepository;
            _floorRepository = floorRepository;
            _areaRepository = areaRepository;
            _contractRepository = contractRepository;
            _paymentContractRepository = paymentContractRepository;

            _entryExitLogRepository = entryExitLogRepository;
        }

        public async Task<List<ParkingLot>> GetPaging(int pageIndex, int pageSize)
        {
            return await _parkinglotRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<ParkingLot> GetById(int id)
        {
            var parkinglot = await _parkinglotRepository.GetById(id);

            if (parkinglot == null) throw AppExceptions.NotFoundParkingLot();

            return parkinglot;
        }

        public async Task<ParkingLot> AddParkingLotAsync(AddParkingLotRequest request)
        {

            var newParkingLot = new ParkingLot
            {
                PricePerDay = request.PricePerDay,
                PricePerHour = request.PricePerHour,
                PricePerMonth = request.PricePerMonth,
                Address = request.Address,
                Lat = request.Lat,
                Long = request.Long,
                OwnerId = request.OwnerId,
                CreatedDate = DateTime.Now.ToVNTime(),
                UpdatedDate = DateTime.Now.ToVNTime()
            };

            newParkingLot.ParkingLotPriceHistories.Add(new ParkingLotPriceHistory
            {
                PricePerHour = newParkingLot.PricePerHour,
                PricePerDay = newParkingLot.PricePerDay,
                PricePerMonth = newParkingLot.PricePerMonth,
                EffectiveDate = newParkingLot.CreatedDate
            });

            return await _parkinglotRepository.Insert(newParkingLot);
        }

        public async Task<ParkingLot> UpdateParkingLotAsync(UpdateParkingLotRequest request)
        {
            var updateParkingLot = await _parkinglotRepository.GetById(request.ParkingLotId);

            if (updateParkingLot == null) throw AppExceptions.NotFoundParkingLot();

            // Giá có thay đổi?
            bool isPriceChanged = false;

            if (updateParkingLot.PricePerHour != request.PricePerHour ||
                updateParkingLot.PricePerDay != request.PricePerDay ||
                updateParkingLot.PricePerMonth != request.PricePerMonth)
            {
                isPriceChanged = true;
            }

            updateParkingLot.PricePerDay = request.PricePerDay;
            updateParkingLot.PricePerHour = request.PricePerHour;
            updateParkingLot.PricePerMonth = request.PricePerMonth;
            updateParkingLot.Address = request.Address;
            updateParkingLot.Lat = request.Lat;
            updateParkingLot.Long = request.Long;

            //Không cập nhật OwnerId và CreatedDate
            //updateParkingLot.OwnerId = 0;
            //updateParkingLot.CreatedDate = DateTime.Now;

            // Thời gian lúc lưu
            updateParkingLot.UpdatedDate = DateTime.Now.ToVNTime();

            // Giá đã đổi
            if (isPriceChanged)
            {
                updateParkingLot.ParkingLotPriceHistories.Add(new ParkingLotPriceHistory
                {
                    PricePerHour = request.PricePerHour,
                    PricePerDay = request.PricePerDay,
                    PricePerMonth = request.PricePerMonth,
                    EffectiveDate = updateParkingLot.UpdatedDate
                });
            }

            await _parkinglotRepository.Update(updateParkingLot);

            return updateParkingLot;
        }

        public async Task<bool> DeleteParkingLotAsync(int id)
        {
            try
            {
                var parkinglot = await _parkinglotRepository.GetById(id);

                if (parkinglot == null) throw AppExceptions.NotFoundParkingLot();

                await _parkinglotRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ParkingLotResponse>> Search(SearchParkingLotRequest request)
        {
            var query = _parkinglotRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.ParkingLotId.ToString().Contains(request.Keyword) ||
                                   (!string.IsNullOrEmpty(x.Address) && x.Address.Contains(request.Keyword)));
            }

            var parkingLots = await query.Select(x => new ParkingLotResponse
            {
                ParkingLotId = x.ParkingLotId,
                Name = $"PL{x.ParkingLotId:0#}",
                Address = x.Address,
                Lat = x.Lat.HasValue ? x.Lat.Value : 0f,
                Long = x.Long.HasValue ? x.Long.Value : 0f,
                PricePerDay = x.PricePerDay,
                PricePerHour = x.PricePerHour,
                PricePerMonth = x.PricePerMonth
            }).ToListAsync();

            return parkingLots;
        }

        public async Task<ParkingLotFullResponse> GetFull(int parkingLotId)
        {
            var parkingLot = await _parkinglotRepository.GetById(parkingLotId);

            if (parkingLot == null) throw AppExceptions.NotFoundParkingLot();

            var areas = await _areaRepository.GetAll()
                                    .Where(x => x.ParkingLotId == parkingLotId)
                                    .Select(x => new AreaResponse
                                    {
                                        AreaId = x.AreaId,
                                        AreaName = x.AreaName,
                                        RentalType = ((RentalType)x.RentalType).ToString()
                                    }).ToListAsync();

            var floors = await _floorRepository.GetAll()
                                    .Include(x => x.Area)
                                    .Where(x => x.Area.ParkingLotId == parkingLotId)
                                    .Select(x => new FloorResponse
                                    {
                                        FloorId = x.FloorId,
                                        FloorName = x.FloorName,
                                        AreaId = x.AreaId
                                    })
                                    .ToListAsync();

            var parkingSpaces = await _parkingSpaceRepository.GetAll()
                            .Include(x => x.Floor).ThenInclude(x => x.Area)
                            .Where(x => x.Floor.Area.ParkingLotId == parkingLotId)
                            .Select(x => new ParkingSpaceResponse
                            {
                                ParkingSpaceId = x.ParkingSpaceId,
                                ParkingSpaceName = x.ParkingSpaceName,
                                FloorId = x.FloorId,
                                Status = ((ParkingSpaceStatus)x.Status).ToString()
                            })
                            .ToListAsync();

            return new ParkingLotFullResponse
            {
                ParkingLot = new ParkingLotResponse
                {
                    ParkingLotId = parkingLot.ParkingLotId,
                    Address = parkingLot.Address,
                    Name = $"PL{parkingLot.ParkingLotId:0#}"
                },
                Areas = areas,
                Floors = floors,
                ParkingSpaces = parkingSpaces
            };
        }

        public async Task<ParkingLotSummaryStatusesResponse> GetSummaryStatuses(int parkingLotId)
        {
            var parkingLot = await _parkinglotRepository.GetById(parkingLotId);

            if (parkingLot == null) throw AppExceptions.NotFoundParkingLot();

            var totalAWalkinSpaces = await _parkingSpaceRepository
                                                .GetAll()
                                                .Include(x => x.Floor).ThenInclude(x => x.Area)
                                                .Where(x => x.Floor.Area.RentalType == (int)RentalType.Walkin &&
                                                            x.Floor.Area.ParkingLotId == parkingLotId &&
                                                            x.Status == (int)ParkingSpaceStatus.Available)
                                                .CountAsync();

            var totalUWalkinSpaces = await _parkingSpaceRepository
                                                .GetAll()
                                                .Include(x => x.Floor).ThenInclude(x => x.Area)
                                                .Where(x => x.Floor.Area.RentalType == (int)RentalType.Walkin && 
                                                            x.Floor.Area.ParkingLotId == parkingLotId && 
                                                            x.Status != (int)ParkingSpaceStatus.Available)
                                                .CountAsync();

            var totalAContractSpaces = await _parkingSpaceRepository
                                                 .GetAll()
                                                 .Include(x => x.Floor).ThenInclude(x => x.Area)
                                                 .Where(x => x.Floor.Area.RentalType == (int)RentalType.Contract &&
                                                             x.Floor.Area.ParkingLotId == parkingLotId &&
                                                             x.Status == (int)ParkingSpaceStatus.Available)
                                                 .CountAsync();

            var totalUContractSpaces = await _parkingSpaceRepository
                                                .GetAll()
                                                .Include(x => x.Floor).ThenInclude(x => x.Area)
                                                .Where(x => x.Floor.Area.RentalType == (int)RentalType.Contract &&
                                                            x.Floor.Area.ParkingLotId == parkingLotId &&
                                                            x.Status != (int)ParkingSpaceStatus.Available)
                                                .CountAsync();

            return new ParkingLotSummaryStatusesResponse
            {
                ParkingLotId = parkingLotId,
                NumberOfAContracts = totalAContractSpaces,
                NumberOfAWalkins = totalAWalkinSpaces,
                NumberOfUContracts = totalUContractSpaces,
                NumberOfUWalkins = totalUWalkinSpaces
            };
        }

        public async Task<IList<ParkingLotResponse>> GetAvailablesForContract(SearchAvailablesForContractRequest request)
        {
            // Validate date range
            if (request.EndDate < request.StartDate)
            {
                throw new ArgumentException("End date cannot be earlier than start date");
            }

            var startDate = new DateOnly(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day);
            var endDate = new DateOnly(request.EndDate.Year, request.EndDate.Month, request.EndDate.Day);

            // Find parking spaces that have active contracts during the requested period
            var contractedParkingSpaceIds = await _contractRepository.GetAll()
                                                    .Where(c => c.Status == (int)ContractStatus.Active &&
                                                              ((c.StartDate <= startDate && c.EndDate >= endDate) ||
                                                               (c.StartDate <= endDate && c.EndDate >= endDate) ||
                                                               (c.StartDate >= startDate && c.EndDate <= endDate)))
                                                    .Select(c => c.ParkingSpaceId)
                                                    .ToListAsync();

            // Find the first available parking lot with available spaces
            var availableParkingLots = await _parkinglotRepository
                                                .GetAll()
                                                .Include(pl => pl.Areas).ThenInclude(a => a.Floors).ThenInclude(f => f.ParkingSpaces)
                                                .Where(pl => pl.Areas.Any(a => a.RentalType == (int)RentalType.Contract && 
                                                                a.Floors.Any(f => f.ParkingSpaces.Any(ps =>
                                                                ps.Status == (int)ParkingSpaceStatus.Available &&
                                                                contractedParkingSpaceIds == null || contractedParkingSpaceIds.Count == 0 || !contractedParkingSpaceIds.Contains(ps.ParkingSpaceId)))))
                                                .Select(pl => new ParkingLotResponse
                                                {
                                                    ParkingLotId = pl.ParkingLotId,
                                                    Name = $"PL{pl.ParkingLotId:0#}",
                                                    Address = pl.Address,
                                                    Lat = pl.Lat.HasValue ? pl.Lat.Value : 0f,
                                                    Long = pl.Long.HasValue ? pl.Long.Value : 0f,
                                                    PricePerDay = pl.PricePerDay,
                                                    PricePerHour = pl.PricePerHour,
                                                    PricePerMonth = pl.PricePerMonth
                                                }).ToListAsync();

            return availableParkingLots;
        }

        public async Task<ParkingLotSummaryResponse> GetSummaries(int parkingLotId)
        {
            // Verify parking lot exists
            var parkingLot = await _parkinglotRepository.GetById(parkingLotId);
            if (parkingLot == null) throw AppExceptions.NotFoundParkingLot();

            // Get total number of areas
            var totalAreas = await _areaRepository.GetAll()
                .Where(a => a.ParkingLotId == parkingLotId)
                .CountAsync();

            // Get total and available parking spaces
            var parkingSpaceStats = await _parkingSpaceRepository.GetAll()
                .Include(ps => ps.Floor)
                    .ThenInclude(f => f.Area)
                .Where(ps => ps.Floor.Area.ParkingLotId == parkingLotId)
                .GroupBy(ps => 1) // Group all results together
                .Select(g => new
                {
                    TotalSpaces = g.Count(),
                    AvailableSpaces = g.Count(ps => ps.Status == (int)ParkingSpaceStatus.Available)
                })
                .FirstOrDefaultAsync() ?? new { TotalSpaces = 0, AvailableSpaces = 0 };

            // Calculate total revenue for today with paid or completed payments
            var today = DateTime.Now.ToVNTime().Date; // Get today's date in VN timezone
            var tomorrow = today.AddDays(1);

            var totalContractRevenue = await _paymentContractRepository
                                                    .GetAll()
                                                    .Where(pc => pc.PaymentDate >= today && pc.PaymentDate < tomorrow) // Filter by today
                                                    .Where(pc => pc.Status == (int)PaymentContractStatus.Paid ||
                                                                 pc.Status == (int)PaymentContractStatus.Completed)
                                                    .Where(pc => pc.Contract.ParkingSpace.Floor.Area.ParkingLotId == parkingLotId)
                                                    .SumAsync(pc => pc.PaymentAmount);

            var totalWalkinRevenue = await _entryExitLogRepository
                                                    .GetAll()
                                                    .Where(log => log.IsPaid == true) // Only paid logs
                                                    .Where(log => log.ExitTime.HasValue && log.ExitTime.Value >= today && log.ExitTime.Value.Date < tomorrow) // Filter by today
                                                    .Where(log => log.ParkingSpace.Floor.Area.ParkingLotId == parkingLotId) // Filter by parking lot
                                                    .SumAsync(log => log.TotalAmount);
            
            return new ParkingLotSummaryResponse
            {
                TotalAreas = totalAreas,
                TotalParkingSpaces = parkingSpaceStats.TotalSpaces,
                TotalAvailableParkingSpaces = parkingSpaceStats.AvailableSpaces,
                TotalRevenues = totalWalkinRevenue + totalContractRevenue
            };
        }

        public async Task<bool> CheckEntranceExisted(string licensePlate)
        {
            return await _entryExitLogRepository.GetAll().AnyAsync(x => x.LicensePlate == licensePlate && x.IsPaid == false);
        }
    }
}
