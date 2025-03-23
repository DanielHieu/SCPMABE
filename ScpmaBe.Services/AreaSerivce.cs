using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services.Models
{
    public  class AreaService : IAreaService
    {  
        private readonly IAreaRepository _areaRepository;

        public AreaService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        public async Task<List<Area>> GetPaging(int pageIndex, int pageSize)
        {
            return await _areaRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Area> GetById(int id)
        {
            var fb = await _areaRepository.GetById(id);

            if (fb == null) throw AppExceptions.NotFoundArea();

            return fb;
        }

        public async Task<Area> AddAreaAsync(AddAreaRequest request)
        {
            var newArea = new Area
            {
               AreaName = request.AreaName,
               ParkingLotId = request.ParkingLotId,
               RentalType = request.RentalType,
               Status = request.Status,
               TotalFloor = 0
            };

            return await _areaRepository.Insert(newArea);
        }

        public async Task<Area> UpdateAreaAsync(UpdateAreaRequest request)
        {
            var updateArea = await _areaRepository.GetById(request.AreaId);

            if (updateArea == null) throw AppExceptions.NotFoundArea();

            updateArea.AreaName = request.AreaName;
            updateArea.Status = request.Status;

            await _areaRepository.Update(updateArea);

            return updateArea;
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

        public async Task<List<Area>> GetAreasByParkingLot(int parkingLotId)
        {
            return await _areaRepository.GetAll().Where(x => x.ParkingLotId == parkingLotId).ToListAsync(); 
        }
    }
}
