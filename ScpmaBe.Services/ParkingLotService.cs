using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public class ParkingLotService : IParkingLotService
    {
        private readonly IParkingLotRepository _parkinglotRepository;

        public ParkingLotService(IParkingLotRepository parkinglotRepository)
        {
            _parkinglotRepository = parkinglotRepository;
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
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
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

            // Không cập nhật OwnerId và CreatedDate
            //updateParkingLot.OwnerId = 0;
            //updateParkingLot.CreatedDate = DateTime.Now;

            // Thời gian lúc lưu
            updateParkingLot.UpdatedDate = DateTime.Now;

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

        public async Task<List<ParkingLot>> Search(SearchParkingLotRequest request)
        {
            var query = _parkinglotRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.ParkingLotId.ToString().Contains(request.Keyword) ||
                                        (!string.IsNullOrEmpty(x.Address) && x.Address.Contains(request.Keyword))
                                    );
            }

            var parkingLots = await query.Select(x => new ParkingLot
            {
                ParkingLotId = x.ParkingLotId,
                OwnerId = x.OwnerId,
                Address = x.Address,
                Lat = x.Lat,
                Long = x.Long,
                PricePerDay = x.PricePerDay,
                PricePerHour = x.PricePerHour,
                PricePerMonth = x.PricePerMonth,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            }).ToListAsync();

            return parkingLots;
        }
    }
}
