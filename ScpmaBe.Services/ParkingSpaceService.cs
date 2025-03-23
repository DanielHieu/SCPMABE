using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class ParkingSpaceService : IParkingSpaceService
    {
        private readonly IParkingSpaceRepository _parkingSpaceRepository;

        public ParkingSpaceService(IParkingSpaceRepository parkingspaceRepository)
        {
            _parkingSpaceRepository = parkingspaceRepository;
        }

        public async Task<List<ParkingSpace>> GetPaging(int pageIndex, int pageSize)
        {
            return await _parkingSpaceRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<ParkingSpace> GetById(int id)
        {
            var parkingspace = await _parkingSpaceRepository.GetById(id);

            if (parkingspace == null) throw AppExceptions.NotFoundParkingSpace();

            return parkingspace;
        }

        public async Task<ParkingSpace> AddParkingSpaceAsync(AddParkingSpaceRequest request)
        {
            var newParkingSpace = new ParkingSpace
            {
                ParkingSpaceName = request.ParkingSpaceName,
                FloorId = request.FloorId,
                Status = request.Status,
            };

            return await _parkingSpaceRepository.Insert(newParkingSpace);
        }

        public async Task<ParkingSpace> UpdateParkingSpaceAsync(UpdateParkingSpaceRequest request)
        {
            var updateParkingSpace = await _parkingSpaceRepository.GetById(request.ParkingSpaceId);
           
            if (updateParkingSpace == null) throw AppExceptions.NotFoundParkingSpace();

            updateParkingSpace.ParkingSpaceName = request.ParkingSpaceName;
            updateParkingSpace.Status = request.Status;

            await _parkingSpaceRepository.Update(updateParkingSpace);

            return updateParkingSpace ;
        }

        public async Task<bool> DeleteParkingSpaceAsync(int id)
        {
            try
            {
                var parkinglot = await _parkingSpaceRepository.GetById(id);

                if (parkinglot == null) throw AppExceptions.NotFoundParkingSpace();

                await _parkingSpaceRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ParkingSpace>> GetParkingSpacesByFloor(int floorId)
        {
            return await _parkingSpaceRepository.GetAll().Where(x => x.FloorId == floorId).ToListAsync();
        }
    }
}

