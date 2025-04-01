using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Common;

namespace ScpmaBe.Services.Models
{
    public class EntryExitLogService : IEntryExitLogService
    {
        private readonly IEntryExitLogRepository _entryExitLogRepository;
        private readonly IParkingSpaceRepository _parkingSpaceRepository;
        private readonly IContractRepository _contractRepository;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public EntryExitLogService(
            ILogger<EntryExitLogService> logger,
            IEntryExitLogRepository entryExitLogRepository,
            IParkingLotRepository parkingLotRepository,
            IParkingSpaceRepository parkingSpaceRepository,
            IContractRepository contractRepository,
            AppSettings appSettings)
        {
            _logger = logger;

            _appSettings = appSettings;

            _entryExitLogRepository = entryExitLogRepository;
            //_parkingLotRepository = parkingLotRepository;
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

            // Store the base64 image to file system
            string entranceImagePath = null;
            if (!string.IsNullOrEmpty(request.EntranceImage))
            {
                try
                {
                    // Create directory if it doesn't exist
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "entrance");
                    Directory.CreateDirectory(directoryPath);

                    // Generate unique filename using license plate and timestamp
                    string fileName = $"{request.LicensePlate}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                    string filePath = Path.Combine(directoryPath, fileName);

                    // Extract actual base64 data (remove "data:image/jpeg;base64," if present)
                    string base64Data = request.EntranceImage;
                    if (base64Data.Contains(","))
                    {
                        base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);
                    }

                    // Convert base64 to bytes and save
                    byte[] imageBytes = Convert.FromBase64String(base64Data);
                    await File.WriteAllBytesAsync(filePath, imageBytes);

                    // Store relative path in database
                    entranceImagePath = $"/files/entrance/{fileName}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save entrance image");
                    // Continue with the process even if image saving fails
                }
            }

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
                    EntranceImage = entranceImagePath ?? request.EntranceImage, // Use file path if available, otherwise use the original base64
                    EntryTime = DateTime.Now.ToVNTime(),
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

        public async Task<SearchResultResponse<EntryExitLogResponse>> Search(SearchEntryExitLogRequest request)
        {
            var totalCount = await _entryExitLogRepository
                                        .GetAll()
                                        .Include(x => x.ParkingSpace)
                                        .Where(x => string.IsNullOrEmpty(request.Keyword) || (!string.IsNullOrEmpty(x.LicensePlate) && x.LicensePlate.Contains(request.Keyword)))
                                        .Where(x => request.ParkingLotId <= 0 || x.ParkingSpace.Floor.Area.ParkingLotId == request.ParkingLotId)
                                        .CountAsync();

            var results = totalCount > 0 ? await _entryExitLogRepository
                                                .GetAll()
                                                .Include(x => x.ParkingSpace)
                                                .Where(x => string.IsNullOrEmpty(request.Keyword) || (!string.IsNullOrEmpty(x.LicensePlate) && x.LicensePlate.Contains(request.Keyword)))
                                                .Where(x => request.ParkingLotId <= 0 || x.ParkingSpace.Floor.Area.ParkingLotId == request.ParkingLotId)
                                                .OrderByDescending(x => (!x.ExitTime.HasValue ? 1: 0))
                                                .ThenByDescending(x => x.ExitTime)
                                                .Skip((request.PageIndex - 1) * request.PageSize)
                                                .Take(request.PageSize)
                                                .Select(x => new EntryExitLogResponse
                                                {
                                                    Id = x.EntryExitLogId,
                                                    LicensePlate = x.LicensePlate,
                                                    EntryTime = x.EntryTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                                    ExitTime = x.ExitTime.HasValue ? x.ExitTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                                    TotalAmount = x.TotalAmount,
                                                    ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                                    ParkingSpaceStatus = ((ParkingSpaceStatus)x.ParkingSpace.Status).ToString(),
                                                    RentalType = ((RentalType)x.RentalType).ToString(),
                                                    IsPaid = x.IsPaid,
                                                    EntranceImage = string.IsNullOrEmpty(x.EntranceImage) ? "" : $"{_appSettings.ApplicationUrl}/{x.EntranceImage}",
                                                    ExitImage = string.IsNullOrEmpty(x.ExitImage) ? "" : $"{_appSettings.ApplicationUrl}/{x.ExitImage}"
                                                }).ToListAsync() :
                                                new List<EntryExitLogResponse>();

            return new SearchResultResponse<EntryExitLogResponse>(totalCount, results) { PageIndex = request.PageIndex, PageSize = request.PageSize };
        }

        public async Task<CalculateFeeResponse> CalculateFeeAsync(CalculateFeeRequest request)
        {
            var entranceEntity = _entryExitLogRepository
                                    .GetAll()
                                    .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                                    .OrderByDescending(x => x.EntryTime)
                                    .FirstOrDefault(x =>
                                        x.IsPaid == false &&
                                        x.LicensePlate == request.LicensePlate &&
                                        x.ParkingSpace.Floor.Area.ParkingLotId == request.ParkingLotId);

            if (entranceEntity == null)
                throw AppExceptions.NotFoundEntryExitLog();

            var exitTime = 
                entranceEntity.ExitTime.HasValue ? entranceEntity.ExitTime.Value :
                DateTime.Now.ToVNTime();

            var contract =
                entranceEntity.RentalType == (int)RentalType.Contract ?
                await _contractRepository
                                    .GetAll()
                                    .Include(x => x.Car).ThenInclude(x => x.Customer)
                                    .Include(x => x.ParkingSpace)
                                    .OrderByDescending(x => x.EndDate)
                                    .Where(x => x.Status.Equals((int)ContractStatus.Active) ||
                                                x.Status.Equals((int)ContractStatus.Expired))
                                    .FirstOrDefaultAsync(x => x.Car.LicensePlate == request.LicensePlate) : null;

            var remainingHour = 0;

            string calculationNotes = "";

            var fee = contract != null ?
                        CalculateContractFee(entranceEntity, contract, exitTime, out remainingHour, out calculationNotes) :
                        CalculateWalkinFee(entranceEntity, exitTime, out calculationNotes);

            // Update the entry log with the calculated fee and exit time
            entranceEntity.TotalAmount = fee;
            entranceEntity.ExitTime = exitTime;

            if (entranceEntity.RentalType == (int)RentalType.Contract)
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
                    ContractId = contract.ContractId,
                    Status = ((ContractStatus)contract.Status).ToString(),
                    StartDate = contract.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = contract.EndDate.ToDateTime(TimeOnly.MaxValue),
                    ParkingSpaceId = contract.ParkingSpaceId,
                    ParkingSpaceName = contract.ParkingSpace.ParkingSpaceName,
                    Car = new CarResponse
                    {
                        LicensePlate = contract.Car.LicensePlate,
                        Model = contract.Car.Model,
                        Color = contract.Car.Color,
                        CustomerName = $"{contract.Car.Customer.FirstName} {contract.Car.Customer.LastName}",
                    }
                } : null,
                LicensePlate = entranceEntity.LicensePlate,
                ParkingSpaceId = entranceEntity.ParkingSpaceId,
                Fee = fee,
                RemainingHour = remainingHour,
                CheckInTime = entranceEntity.EntryTime,
                CheckOutTime = exitTime,
                CalculationNotes = calculationNotes,
                EntranceImage = string.IsNullOrEmpty(entranceEntity.EntranceImage) ? "" : $"{_appSettings.ApplicationUrl}/{entranceEntity.EntranceImage}",
            };
        }

        private decimal CalculateWalkinFee(EntryExitLog entranceEntity, DateTime exitTime, out string calculationNotes)
        {
            // Calculate time difference
            TimeSpan parkingDuration = exitTime - entranceEntity.EntryTime;

            decimal fee = 0;

            System.Text.StringBuilder notes = new System.Text.StringBuilder();

            notes.AppendLine($"Tính phí vãng lai cho biển số xe: {entranceEntity.LicensePlate}");
            notes.AppendLine($"Thời gian vào: {entranceEntity.EntryTime:yyyy-MM-dd HH:mm:ss}");
            notes.AppendLine($"Thời gian ra: {exitTime:yyyy-MM-dd HH:mm:ss}");
            notes.AppendLine($"Tổng thời gian: {parkingDuration.Days} ngày, {parkingDuration.Hours} giờ, {parkingDuration.Minutes} phút");
            notes.AppendLine($"Giá theo giờ: {entranceEntity.PricePerHour} VNĐ");
            notes.AppendLine($"Giá theo ngày: {entranceEntity.PricePerDay} VNĐ");

            // Calculate complete days
            int completeDays = (int)Math.Floor(parkingDuration.TotalDays);

            // Calculate remaining hours after complete days
            int remainingHours = (int)Math.Ceiling(parkingDuration.TotalHours - (completeDays * 24));

            // Apply daily rate for complete days
            if (completeDays > 0)
            {
                decimal dailyFee = completeDays * entranceEntity.PricePerDay;
                fee += dailyFee;
                notes.AppendLine($"Phí theo ngày: {completeDays} × {entranceEntity.PricePerDay} VNĐ = {dailyFee} VNĐ");
            }

            // Apply hourly rate for remaining hours
            if (remainingHours > 0)
            {
                // If remaining hours cost more than a day rate, cap it at day rate
                decimal hourlyFee = remainingHours * entranceEntity.PricePerHour;
                if (hourlyFee > entranceEntity.PricePerDay)
                {
                    fee += entranceEntity.PricePerDay;
                    notes.AppendLine($"Số giờ còn lại: {remainingHours} × {entranceEntity.PricePerHour} VNĐ = {hourlyFee} VNĐ");
                    notes.AppendLine($"Phí theo giờ được giới hạn ở mức phí theo ngày: {entranceEntity.PricePerDay} VNĐ");
                }
                else
                {
                    fee += hourlyFee;
                    notes.AppendLine($"Số giờ còn lại: {remainingHours} × {entranceEntity.PricePerHour} VNĐ = {hourlyFee} VNĐ");
                }
            }

            // If total fee is 0 (e.g., for very short stays), charge minimum of 1 hour
            if (fee == 0 && parkingDuration.TotalMinutes > 0)
            {
                fee = entranceEntity.PricePerHour;
                notes.AppendLine($"Áp dụng mức phí tối thiểu 1 giờ: {entranceEntity.PricePerHour} VNĐ");
            }

            notes.AppendLine($"Tổng phí: {fee:C}");
            calculationNotes = notes.ToString();
            return fee;
        }

        private decimal CalculateContractFee(EntryExitLog entranceEntity, Contract contract, DateTime exitTime, out int remainingHour, out string calculationNotes)
        {
            remainingHour = 0;

            System.Text.StringBuilder notes = new System.Text.StringBuilder();

            notes.AppendLine($"Tính phí hợp đồng cho biển số xe: {entranceEntity.LicensePlate}");
            notes.AppendLine($"Thời hạn hợp đồng: {contract.StartDate:yyyy-MM-dd} đến {contract.EndDate:yyyy-MM-dd}");
            notes.AppendLine($"Thời gian vào: {entranceEntity.EntryTime:yyyy-MM-dd HH:mm:ss}");
            notes.AppendLine($"Thời gian ra: {exitTime:yyyy-MM-dd HH:mm:ss}");

            // If exit time is within contract period, no additional fee
            if (exitTime.Date <= contract.EndDate.ToDateTime(TimeOnly.MaxValue))
            {
                notes.AppendLine("Thời gian ra nằm trong thời hạn hợp đồng - Không phát sinh phí thêm");
                calculationNotes = notes.ToString();
                return 0;
            }

            // If exit time is after contract end date, calculate additional fee as walk-in
            notes.AppendLine("Thời gian ra vượt quá thời hạn hợp đồng - Tính phí thêm như khách vãng lai");

            // First, create a version of the entry log with entry time set to end of contract
            var walkInEntry = new EntryExitLog
            {
                LicensePlate = entranceEntity.LicensePlate,
                EntryTime = contract.EndDate.ToDateTime(TimeOnly.MinValue),
                PricePerDay = entranceEntity.PricePerDay,
                PricePerHour = entranceEntity.PricePerHour,
                PricePerMonth = entranceEntity.PricePerMonth
            };

            remainingHour = (int)(exitTime - contract.EndDate.ToDateTime(TimeOnly.MaxValue)).TotalHours;
            notes.AppendLine($"Số giờ vượt quá hợp đồng: {remainingHour}");

            // Calculate the walk-in fee for time beyond contract
            string walkInNotes;
            decimal fee = CalculateWalkinFee(walkInEntry, exitTime, out walkInNotes);

            notes.AppendLine("Chi tiết tính phí thêm:");
            notes.AppendLine(walkInNotes);

            calculationNotes = notes.ToString();

            return fee;
        }

        public async Task<List<EntrancingCarResponse>> GetEntrancingCars(int parkingLotId)
        {
            var entranceEntities = await _entryExitLogRepository
                                            .GetAll()
                                            .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                                            .Where(x => !x.IsPaid && x.ParkingSpace.Floor.Area.ParkingLotId == parkingLotId)
                                            .OrderByDescending(x => x.EntryTime)
                                            .Select(x => new EntrancingCarResponse
                                            {
                                                Id = x.EntryExitLogId,
                                                LicensePlate = x.LicensePlate,
                                                CheckInTime = x.EntryTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                                CheckOutTime = x.ExitTime.HasValue ? x.ExitTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                                AreaName = x.ParkingSpace.Floor.Area.AreaName,
                                                FloorName = x.ParkingSpace.Floor.FloorName,
                                                ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                                RentalType = ((RentalType)x.RentalType).ToString()
                                            })
                                            .ToListAsync();

            return entranceEntities;
        }

        public async Task<bool> Pay(int id, string exitImage)
        {
            var entryExitLog = await _entryExitLogRepository.GetById(id);

            if (entryExitLog == null) throw AppExceptions.NotFoundEntryExitLog();

            // Store the base64 exit image to file system
            string exitImagePath = null;
            if (!string.IsNullOrEmpty(exitImage))
            {
                try
                {
                    // Create directory if it doesn't exist
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "exit");
                    Directory.CreateDirectory(directoryPath);

                    // Generate unique filename using license plate and timestamp
                    string fileName = $"{entryExitLog.LicensePlate}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                    string filePath = Path.Combine(directoryPath, fileName);

                    // Extract actual base64 data (remove "data:image/jpeg;base64," if present)
                    string base64Data = exitImage;
                    if (base64Data.Contains(","))
                    {
                        base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);
                    }

                    // Convert base64 to bytes and save
                    byte[] imageBytes = Convert.FromBase64String(base64Data);
                    await File.WriteAllBytesAsync(filePath, imageBytes);

                    // Store relative path in database
                    exitImagePath = $"/files/exit/{fileName}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save exit image");
                    // Continue with the process even if image saving fails
                }
            }

            entryExitLog.IsPaid = true;
            entryExitLog.ExitImage = exitImagePath ?? ""; // Use file path if available

            var parkingSpace = await _parkingSpaceRepository.GetById(entryExitLog.ParkingSpaceId);

            if (parkingSpace == null) throw AppExceptions.NotFoundParkingSpace();

            if (entryExitLog.RentalType == (int)RentalType.Contract)
                parkingSpace.Status = entryExitLog.TotalAmount > 0 ?
                                        (int)ParkingSpaceStatus.Available :
                                        (int)ParkingSpaceStatus.Reserved;
            else
                parkingSpace.Status = (int)ParkingSpaceStatus.Available;

            await _entryExitLogRepository.Update(entryExitLog);

            return true;
        }
    }
}


