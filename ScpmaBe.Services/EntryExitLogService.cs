using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public class EntryExitLogService : IEntryExitLogService
    {
        private readonly IEntryExitLogRepository _entryExitLogRepository;

        public EntryExitLogService(IEntryExitLogRepository entryExitLogRepository)
        {
            _entryExitLogRepository = entryExitLogRepository;
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
            var newEntryExitLog = new EntryExitLog
            {
                LicensePlate = request.LicensePlate,
                ParkingSpaceId = request.ParkingSpaceId,
                PricePerDay = request.PricePerDay,
                PricePerHour = request.PricePerHour,
                PricePerMonth = request.PricePerMonth,
                RentalType = request.RentalType,
                EntryTime = DateTime.Now,
                TotalAmount = 0
            };

            return await _entryExitLogRepository.Insert(newEntryExitLog);
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
    }
}


