using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IAreaRepository _areaRepository;

        public FloorService(IFloorRepository floorRepository, IAreaRepository areaRepository)
        {
            _floorRepository = floorRepository;
            _areaRepository = areaRepository;
        }

        public async Task<Floor> GetById(int id)
        {
            var fb = await _floorRepository.GetById(id);

            if (fb == null) throw AppExceptions.NotFoundFloor();

            return fb;
        }

        public async Task<FloorResponse> AddFloorAsync(AddFloorRequest request)
        {
            var newFloor = new Floor
            {
                FloorName = request.FloorName,
                AreaId = request.AreaId,
                TotalParkingSpace = 0,
                Status = 0,
            };

            var area = await _areaRepository.GetById(request.AreaId);

            if (area == null) throw AppExceptions.NotFoundArea();

            var nameExisted = await _floorRepository.GetAll().AnyAsync(x => x.FloorName.ToLower() == request.FloorName.ToLower() && x.AreaId == request.AreaId);

            if (nameExisted) throw AppExceptions.FloorNameExisted();

            area.TotalFloor++;

            var addedEntity = await _floorRepository.Insert(newFloor);

            return new FloorResponse
            {
                FloorId = addedEntity.FloorId,
                FloorName = addedEntity.FloorName,
                AreaId = addedEntity.AreaId,
                TotalParkingSpaces = addedEntity.TotalParkingSpace
            };
        }

        public async Task<FloorResponse> UpdateFloorAsync(UpdateFloorRequest request)
        {
            var updateFloor = await _floorRepository.GetById(request.FloorId);

            if (updateFloor == null) throw AppExceptions.NotFoundFloor();

            var areaId = updateFloor.AreaId;

            var nameExisted = await _floorRepository.GetAll().AnyAsync(x => x.FloorName.ToLower() == request.FloorName.ToLower() && x.AreaId == areaId && x.FloorId != request.FloorId);

            if (nameExisted) throw AppExceptions.FloorNameExisted();

            updateFloor.FloorName = request.FloorName;

            await _floorRepository.Update(updateFloor);

            return new FloorResponse
            {
                FloorId = updateFloor.FloorId,
                FloorName = updateFloor.FloorName,
                AreaId = updateFloor.AreaId,
                TotalParkingSpaces = updateFloor.TotalParkingSpace
            };
        }

        public async Task<bool> DeleteFloorAsync(int id)
        {
            try
            {
                var deleteFloor = await _floorRepository.GetById(id);

                if (deleteFloor == null) throw AppExceptions.NotFoundFloor();

                var area = await _areaRepository.GetById(deleteFloor.AreaId);

                if (area == null) throw AppExceptions.NotFoundArea();

                area.TotalFloor--;

                await _floorRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<FloorResponse>> GetFloorsByArea(int areaId)
        {
            return await _floorRepository.GetAll()
                                .Where(x => x.AreaId == areaId)
                                .Select(x=> new FloorResponse
                                {
                                    FloorId = x.FloorId,
                                    FloorName = x.FloorName,
                                    AreaId = x.AreaId,
                                    TotalParkingSpaces = x.TotalParkingSpace
                                }).ToListAsync();
        }

        public async Task<List<FloorResponse>> GetFloorsByParkingLot(int parkingLotId)
        {
            return await _floorRepository
                            .GetAll()
                            .Include(x => x.Area)
                            .Where(x => x.Area.ParkingLotId == parkingLotId)
                            .Select(x => new FloorResponse
                            {
                                FloorId = x.FloorId,
                                FloorName = x.FloorName,
                                AreaId = x.AreaId,
                                TotalParkingSpaces = x.TotalParkingSpace
                            })
                            .ToListAsync();
        }
    }
}
