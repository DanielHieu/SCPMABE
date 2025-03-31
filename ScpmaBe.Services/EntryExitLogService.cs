using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public class EntryExitLogService : IEntryExitLogService
    {
        private readonly IEntryExitLogRepository _entryExitLogRepository;
        private readonly IParkingLotRepository _parkingLotRepository;
        private readonly IParkingSpaceRepository _parkingSpaceRepository;
        private readonly IContractRepository _contractRepository;

        private readonly ILogger _logger;

        public EntryExitLogService(
            ILogger<EntryExitLogService> logger,
            IEntryExitLogRepository entryExitLogRepository,
            IParkingLotRepository parkingLotRepository,
            IParkingSpaceRepository parkingSpaceRepository,
            IContractRepository contractRepository)
        {
            _logger = logger;

            _entryExitLogRepository = entryExitLogRepository;

            _parkingLotRepository = parkingLotRepository;
            _parkingSpaceRepository = parkingSpaceRepository;

            _contractRepository = contractRepository;
        }

        public async Task<List<EntryExitLog>> GetPaging(int pageIndex, int pageSize)
        {
            return await _entryExitLogRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<EntryExitLog> GetById(int id)
        {
            var entity = await _entryExitLogRepository.GetById(id);

            if (entity == null) throw AppExceptions.NotFoundEntryExitLog();

            return entity;
        }

        public async Task<EntryExitLog> AddEntryExitLogAsync(AddEntryExitLogRequest request)
        {
            var parkingSpace = await _parkingSpaceRepository
                                        .GetAll()
                                        .Include(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                        .FirstOrDefaultAsync(x => x.ParkingSpaceId == request.ParkingSpaceId);

            if (parkingSpace == null)
                throw AppExceptions.NotFoundParkingSpace();

            try
            {
                var parkingLot = parkingSpace.Floor.Area.ParkingLot;

                var newEntryExitLog = new EntryExitLog
                {
                    LicensePlate = request.LicensePlate,
                    ParkingSpaceId = request.ParkingSpaceId,
                    PricePerDay = parkingLot.PricePerDay,
                    PricePerHour = parkingLot.PricePerHour,
                    PricePerMonth = parkingLot.PricePerMonth,
                    RentalType = request.RentalType,
                    EntryTime = DateTime.Now,
                    ExitTime = null,
                    TotalAmount = 0
                };

                parkingSpace.Status = (int)ParkingSpaceStatus.Pending;

                var entity = await _entryExitLogRepository.Insert(newEntryExitLog);

                return new EntryExitLog
                {
                    EntryTime = entity.EntryTime,
                    LicensePlate = entity.LicensePlate,
                    EntryExitLogId = entity.EntryExitLogId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw; // Re-throw the exception to maintain the method's contract
            }
        }

        public async Task<EntryExitLog> UpdateEntryExitLogAsync(UpdateEntryExitLogRequest request)
        {
            var updateEntryExitLog = await _entryExitLogRepository.GetById(request.EntryExitLogId);

            if (updateEntryExitLog == null) throw AppExceptions.NotFoundEntryExitLog();

            updateEntryExitLog.ExitTime = request.ExitTime;
            updateEntryExitLog.TotalAmount = request.TotalAmount;

            await _entryExitLogRepository.Update(updateEntryExitLog);

            return updateEntryExitLog;
        }

        public async Task<bool> DeleteEntryExitLogAsync(int id)
        {
            try
            {
                var deleteEntryExitLog = await _entryExitLogRepository.GetById(id);

                if (deleteEntryExitLog == null) throw AppExceptions.NotFoundEntryExitLog();

                await _entryExitLogRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<EntryExitLog>> Search(SearchEntryExitLogRequest request)
        {
            return await _entryExitLogRepository
                                        .GetAll()
                                        .Where(x => !string.IsNullOrEmpty(x.LicensePlate) && x.LicensePlate.Contains(request.Keyword))
                                        .Select(x => new EntryExitLog
                                        {
                                            EntryExitLogId = x.EntryExitLogId,
                                            ParkingSpaceId = x.ParkingSpaceId,
                                            LicensePlate = x.LicensePlate,
                                            EntryTime = x.EntryTime,
                                            ExitTime = x.ExitTime,
                                            PricePerDay = x.PricePerDay,
                                            PricePerHour = x.PricePerHour,
                                            PricePerMonth = x.PricePerMonth,
                                            RentalType = x.RentalType,
                                            TotalAmount = x.TotalAmount
                                        })
                                        .ToListAsync();
        }
        public async Task<CalculateFeeResponse> CalculateFeeAsync(CalculateFeeRequest request)
        {
            var entranceEntity = _entryExitLogRepository
                                    .GetAll()
                                    .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                                    .OrderByDescending(x => x.EntryTime)
                                    .FirstOrDefault(x =>
                                        x.ExitTime == null &&
                                        x.LicensePlate == request.LicensePlate &&
                                        x.ParkingSpace.Floor.Area.ParkingLotId == request.ParkingLotId);

            if (entranceEntity == null)
                throw AppExceptions.NotFoundEntryExitLog();

            var exitTime = DateTime.Now;

            var contract = await _contractRepository
                                    .GetAll()
                                    .OrderByDescending(x => x.EndDate)
                                    .Where(x => x.Status.Equals((int)ContractStatus.Active) ||
                                                x.Status.Equals((int)ContractStatus.Expired))
                                    .FirstOrDefaultAsync(x => x.Car.LicensePlate == request.LicensePlate);

            var remainingHour = 0;

            string calculationNotes = "";

            var fee = contract != null && entranceEntity.RentalType == (int)RentalType.Contract ?
                        CalculateContractFee(entranceEntity, contract, exitTime, out remainingHour, out calculationNotes) :
                        CalculateWalkinFee(entranceEntity, exitTime, out calculationNotes);

            // Update the entry log with the calculated fee and exit time
            entranceEntity.TotalAmount = fee;
            entranceEntity.ExitTime = exitTime;

            if(entranceEntity.RentalType == (int)RentalType.Contract)
                entranceEntity.ParkingSpace.Status = (int)ParkingSpaceStatus.Pending;
            else 
                entranceEntity.ParkingSpace.Status = (int)ParkingSpaceStatus.Available;

            await _entryExitLogRepository.Update(entranceEntity);

            return new CalculateFeeResponse
            {
                Id = entranceEntity.EntryExitLogId,
                RentalType = ((RentalType)entranceEntity.RentalType).ToString(),
                Contract = contract != null ? new ContractResponse
                {
                    Status = ((ContractStatus)contract.Status),
                    StartDate = contract.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = contract.EndDate.ToDateTime(TimeOnly.MaxValue),
                    Id = contract.ContractId,
                    ParkingSpaceId = contract.ParkingSpaceId,
                    LicensePlate = contract.Car.LicensePlate
                } : null,
                LicensePlate = entranceEntity.LicensePlate,
                ParkingSpaceId = entranceEntity.ParkingSpaceId,
                Fee = fee,
                RemainingHour = remainingHour,
                CheckInTime = entranceEntity.EntryTime,
                CheckOutTime = exitTime,
                CalculationNotes = calculationNotes
            };
        }

        private decimal CalculateWalkinFee(EntryExitLog entranceEntity, DateTime exitTime, out string calculationNotes)
        {
            // Calculate time difference
            TimeSpan parkingDuration = exitTime - entranceEntity.EntryTime;

            decimal fee = 0;

            System.Text.StringBuilder notes = new System.Text.StringBuilder();

            notes.AppendLine($"Walk-in fee calculation for license plate: {entranceEntity.LicensePlate}");
            notes.AppendLine($"Entry time: {entranceEntity.EntryTime:yyyy-MM-dd HH:mm:ss}");
            notes.AppendLine($"Exit time: {exitTime:yyyy-MM-dd HH:mm:ss}");
            notes.AppendLine($"Total duration: {parkingDuration.Days} days, {parkingDuration.Hours} hours, {parkingDuration.Minutes} minutes");
            notes.AppendLine($"Price per hour: {entranceEntity.PricePerHour:C}");
            notes.AppendLine($"Price per day: {entranceEntity.PricePerDay:C}");

            // Calculate complete days
            int completeDays = (int)Math.Floor(parkingDuration.TotalDays);

            // Calculate remaining hours after complete days
            int remainingHours = (int)Math.Ceiling(parkingDuration.TotalHours - (completeDays * 24));

            // Apply daily rate for complete days
            if (completeDays > 0)
            {
                decimal dailyFee = completeDays * entranceEntity.PricePerDay;
                fee += dailyFee;
                notes.AppendLine($"Days charged: {completeDays} × {entranceEntity.PricePerDay:C} = {dailyFee:C}");
            }

            // Apply hourly rate for remaining hours
            if (remainingHours > 0)
            {
                // If remaining hours cost more than a day rate, cap it at day rate
                decimal hourlyFee = remainingHours * entranceEntity.PricePerHour;
                if (hourlyFee > entranceEntity.PricePerDay)
                {
                    fee += entranceEntity.PricePerDay;
                    notes.AppendLine($"Remaining hours: {remainingHours} × {entranceEntity.PricePerHour:C} = {hourlyFee:C}");
                    notes.AppendLine($"Hourly fee capped at daily rate: {entranceEntity.PricePerDay:C}");
                }
                else
                {
                    fee += hourlyFee;
                    notes.AppendLine($"Remaining hours: {remainingHours} × {entranceEntity.PricePerHour:C} = {hourlyFee:C}");
                }
            }

            // If total fee is 0 (e.g., for very short stays), charge minimum of 1 hour
            if (fee == 0 && parkingDuration.TotalMinutes > 0)
            {
                fee = entranceEntity.PricePerHour;
                notes.AppendLine($"Minimum charge of 1 hour applied: {entranceEntity.PricePerHour:C}");
            }

            notes.AppendLine($"Total fee: {fee:C}");
            calculationNotes = notes.ToString();
            return fee;
        }

        private decimal CalculateContractFee(EntryExitLog entranceEntity, Contract contract, DateTime exitTime, out int remainingHour, out string calculationNotes)
        {
            remainingHour = 0;
            System.Text.StringBuilder notes = new System.Text.StringBuilder();
            notes.AppendLine($"Contract fee calculation for license plate: {entranceEntity.LicensePlate}");
            notes.AppendLine($"Contract period: {contract.StartDate:yyyy-MM-dd} to {contract.EndDate:yyyy-MM-dd}");
            notes.AppendLine($"Entry time: {entranceEntity.EntryTime:yyyy-MM-dd HH:mm:ss}");
            notes.AppendLine($"Exit time: {exitTime:yyyy-MM-dd HH:mm:ss}");

            // If exit time is within contract period, no additional fee
            if (exitTime.Date <= contract.EndDate.ToDateTime(TimeOnly.MinValue))
            {
                notes.AppendLine("Exit time is within contract period - No additional fee");
                calculationNotes = notes.ToString();
                return 0;
            }

            // If exit time is after contract end date, calculate additional fee as walk-in
            notes.AppendLine("Exit time is after contract end date - Additional fee calculated as walk-in");

            // First, create a version of the entry log with entry time set to end of contract
            var walkInEntry = new EntryExitLog
            {
                LicensePlate = entranceEntity.LicensePlate,
                EntryTime = contract.EndDate.ToDateTime(TimeOnly.MaxValue), // End of the last contract day
                PricePerDay = entranceEntity.PricePerDay,
                PricePerHour = entranceEntity.PricePerHour,
                PricePerMonth = entranceEntity.PricePerMonth
            };

            remainingHour = (int)(exitTime - contract.EndDate.ToDateTime(TimeOnly.MinValue)).TotalHours;
            notes.AppendLine($"Hours beyond contract: {remainingHour}");

            // Calculate the walk-in fee for time beyond contract
            string walkInNotes;
            decimal fee = CalculateWalkinFee(walkInEntry, exitTime, out walkInNotes);

            notes.AppendLine("Additional walk-in fee calculation:");
            notes.AppendLine(walkInNotes);
            calculationNotes = notes.ToString();

            return fee;
        }


        public async Task<List<EntrancingCarResponse>> GetEntrancingCars(int parkingLotId)
        {
            var entranceEntities = await _entryExitLogRepository
                                            .GetAll()
                                            .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                                            .Where(x => !x.ExitTime.HasValue && x.ParkingSpace.Floor.Area.ParkingLotId == parkingLotId)
                                            .Select(x => new EntrancingCarResponse
                                            {
                                                Id = x.EntryExitLogId,
                                                LicensePlate = x.LicensePlate,
                                                CheckInTime = x.EntryTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                                CheckOutTime = x.ExitTime.HasValue ? x.ExitTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                                AreaName = x.ParkingSpace.Floor.Area.AreaName,
                                                FloorName = x.ParkingSpace.Floor.FloorName,
                                                ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                            })
                                            .ToListAsync();

            return entranceEntities;
        }
    }
}


