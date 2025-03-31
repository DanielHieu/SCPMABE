using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public  class FloorService : IFloorService
    {  
        private readonly IFloorRepository _floorRepository;

        public FloorService(IFloorRepository floorRepository)
        {
            _floorRepository = floorRepository;
        }

        public async Task<List<Floor>> GetPaging(int pageIndex, int pageSize)
        {
            return await _floorRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Floor> GetById(int id)
        {
            var fb = await _floorRepository.GetById(id);

            if (fb == null) throw AppExceptions.NotFoundFloor();

            return fb;
        }

        public async Task<Floor> AddFloorAsync(AddFloorRequest request)
        {
            var newFloor = new Floor
            {
               FloorName = request.FloorName,
               AreaId = request.AreaId,
               NumberEmptyParkingSpace = 0,
               NumberUsedParkingSpace = 0,
               TotalParkingSpace = 0,
               Status = 0,
            };

            return await _floorRepository.Insert(newFloor);
        }

        public async Task<Floor> UpdateFloorAsync(UpdateFloorRequest request)
        {
            var updateFloor = await _floorRepository.GetById(request.FloorId);

            if (updateFloor == null) throw AppExceptions.NotFoundFloor();

            updateFloor.FloorName = request.FloorName;
            updateFloor.Status = request.Status;

            await _floorRepository.Update(updateFloor);

            return updateFloor;
        }

        public async Task<bool> DeleteFloorAsync(int id)
        {
            try
            {
                var deleteFloor = await _floorRepository.GetById(id);

                if (deleteFloor == null) throw AppExceptions.NotFoundFloor();

                await _floorRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Floor>> GetFloorsByArea(int areaId)
        {
            return await _floorRepository.GetAll().Where(x => x.AreaId == areaId).ToListAsync();
        }

        public async Task<List<Floor>> GetFloorsByParkingLot(int parkingLotId)
        {
            return await _floorRepository
                            .GetAll()
                            .Include(x => x.Area)
                            .Where(x => x.Area.ParkingLotId == parkingLotId)
                            .Select(x=> new Floor
                            {
                                FloorId = x.FloorId,
                                FloorName = x.FloorName,
                                AreaId = x.AreaId,
                                NumberEmptyParkingSpace = x.NumberEmptyParkingSpace,
                                NumberUsedParkingSpace = x.NumberUsedParkingSpace,
                                TotalParkingSpace = x.TotalParkingSpace,
                                Status = x.Status
                            })
                            .ToListAsync();
        }
    }
}
