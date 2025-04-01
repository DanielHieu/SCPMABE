using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;

        public AreaService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        public async Task<AreaResponse> GetById(int id)
        {
            var entity = await _areaRepository.GetById(id);

            if (entity == null) throw AppExceptions.NotFoundArea();

           return new AreaResponse
           {
               AreaId = entity.AreaId,
               AreaName = entity.AreaName,
               RentalType = ((RentalType)entity.RentalType).ToString(),
               TotalFloors = entity.TotalFloor
           };
        }

        public async Task<AreaResponse> AddAreaAsync(AddAreaRequest request)
        {
            var nameExisted = await _areaRepository.GetAll().AnyAsync(x => x.AreaName == request.AreaName && x.ParkingLotId == request.ParkingLotId);

            if (nameExisted) throw AppExceptions.AreaNameExisted();

            var newArea = new Area
            {
                AreaName = request.AreaName,
                ParkingLotId = request.ParkingLotId,
                RentalType = (int)request.RentalType,
                Status = 1,
                TotalFloor = 0
            };

            var newEntity = await _areaRepository.Insert(newArea);

            return new AreaResponse
            {
                AreaId = newEntity.AreaId,
                AreaName = newEntity.AreaName,
                RentalType = ((RentalType)newEntity.RentalType).ToString(),
                TotalFloors = newEntity.TotalFloor
            };
        }

        public async Task<AreaResponse> UpdateAreaAsync(UpdateAreaRequest request)
        {
            var updateArea = await _areaRepository.GetById(request.AreaId);

            if (updateArea == null) throw AppExceptions.NotFoundArea();

            var nameExisted = await _areaRepository.GetAll().AnyAsync(x => x.AreaName == request.AreaName && x.ParkingLotId == updateArea.ParkingLotId && x.AreaId != request.AreaId);

            if (nameExisted) throw AppExceptions.AreaNameExisted();

            updateArea.AreaName = request.AreaName;

            await _areaRepository.Update(updateArea);

            return new AreaResponse
            {
                AreaId = updateArea.AreaId,
                AreaName = updateArea.AreaName,
                RentalType = ((RentalType)updateArea.RentalType).ToString(),
                TotalFloors = updateArea.TotalFloor
            };
        }

        public async Task<bool> DeleteAreaAsync(int id)
        {
            try
            {
                var deleteArea = await _areaRepository.GetById(id);

                if (deleteArea == null) throw AppExceptions.NotFoundArea();

                await _areaRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AreaResponse>> GetAreasByParkingLot(int parkingLotId)
        {
            return await _areaRepository.GetAll()
                            .Where(x => x.ParkingLotId == parkingLotId)
                            .Select(x => new AreaResponse
                            {
                                AreaId = x.AreaId,
                                AreaName = x.AreaName,
                                RentalType = ((RentalType)x.RentalType).ToString(),
                                TotalFloors = x.TotalFloor,
                            })
                            .ToListAsync();
        }
    }
}
