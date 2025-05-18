using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class ParkingSpaceService : IParkingSpaceService
    {
        private readonly IParkingSpaceRepository _parkingSpaceRepository;
        private readonly IParkingStatusSensorRepository _parkingStatusSensorRepository;
        private readonly IFloorRepository _floorRepository;

        public ParkingSpaceService(
            IParkingSpaceRepository parkingspaceRepository, IParkingStatusSensorRepository parkingStatusSensorRepository, IFloorRepository floorRepository)
        {
            _parkingSpaceRepository = parkingspaceRepository;
            _parkingStatusSensorRepository = parkingStatusSensorRepository;
            _floorRepository = floorRepository;
        }

        public async Task<List<ParkingSpace>> GetPaging(int pageIndex, int pageSize)
        {
            return await _parkingSpaceRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<ParkingSpaceResponse> GetById(int id)
        {
            var parkingspace = await _parkingSpaceRepository.GetById(id);

            if (parkingspace == null) throw AppExceptions.NotFoundParkingSpace();

            return new ParkingSpaceResponse
            {
                ParkingSpaceId = parkingspace.ParkingSpaceId,
                FloorId = parkingspace.FloorId,
                ParkingSpaceName = parkingspace.ParkingSpaceName,
                Status = ((ParkingSpaceStatus)parkingspace.Status).ToString()
            };
        }

        public async Task<ParkingSpaceResponse> AddParkingSpaceAsync(AddParkingSpaceRequest request)
        {
            var newParkingSpace = new ParkingSpace
            {
                ParkingSpaceName = request.ParkingSpaceName,
                FloorId = request.FloorId,
                Status = 1,
            };

            var floor = await _floorRepository.GetById(request.FloorId);

            if (floor == null) throw AppExceptions.NotFoundFloor();

            var nameExisted = await _parkingSpaceRepository.GetAll().AnyAsync(x => x.ParkingSpaceName.ToLower() == request.ParkingSpaceName.ToLower() && x.FloorId == request.FloorId);

            if(nameExisted) throw AppExceptions.ParkingSpaceNameExisted();

            floor.TotalParkingSpace++;

            var addedEntity = await _parkingSpaceRepository.Insert(newParkingSpace);

            return new ParkingSpaceResponse
            {
                ParkingSpaceId = addedEntity.ParkingSpaceId,
                FloorId = addedEntity.FloorId,
                ParkingSpaceName = addedEntity.ParkingSpaceName,
                Status = ((ParkingSpaceStatus)addedEntity.Status).ToString()
            };
        }

        public async Task<ParkingSpaceResponse> UpdateParkingSpaceAsync(UpdateParkingSpaceRequest request)
        {
            var updateParkingSpace = await _parkingSpaceRepository.GetById(request.ParkingSpaceId);

            if (updateParkingSpace == null) throw AppExceptions.NotFoundParkingSpace();

            var floorId = updateParkingSpace.FloorId;

            var nameExisted = await _parkingSpaceRepository.GetAll().AnyAsync(x => x.ParkingSpaceName.ToLower() == request.ParkingSpaceName.ToLower() && x.FloorId == floorId && x.ParkingSpaceId != request.ParkingSpaceId);

            if(nameExisted) throw AppExceptions.ParkingSpaceNameExisted();

            updateParkingSpace.ParkingSpaceName = request.ParkingSpaceName;

            await _parkingSpaceRepository.Update(updateParkingSpace);

            return new ParkingSpaceResponse
            {
                ParkingSpaceId = updateParkingSpace.ParkingSpaceId,
                FloorId = updateParkingSpace.FloorId,
                ParkingSpaceName = updateParkingSpace.ParkingSpaceName,
                Status = ((ParkingSpaceStatus)updateParkingSpace.Status).ToString()
            };
        }

        public async Task<bool> DeleteParkingSpaceAsync(int id)
        {
            try
            {
                var parkinglot = await _parkingSpaceRepository.GetById(id);

                if (parkinglot == null) throw AppExceptions.NotFoundParkingSpace();

                var floor = await _floorRepository.GetById(parkinglot.FloorId);

                if (floor == null) throw AppExceptions.NotFoundFloor();

                floor.TotalParkingSpace--;

                await _parkingSpaceRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ParkingSpaceResponse>> GetParkingSpacesByFloor(int floorId)
        {
            return await _parkingSpaceRepository
                            .GetAll()
                            .Where(x => x.FloorId == floorId)
                            .Select(x => new ParkingSpaceResponse
                            {
                                ParkingSpaceId = x.ParkingSpaceId,
                                ParkingSpaceName = x.ParkingSpaceName,
                                FloorId = x.FloorId,
                                Status = ((ParkingSpaceStatus)x.Status).ToString()
                            }).ToListAsync();
        }

        public async Task<bool> ChangeStatus(string apiKey)
        {
            var parkingStatusSensor = await _parkingStatusSensorRepository.GetAll().FirstOrDefaultAsync(x => x.ApiKey == apiKey);

            if (parkingStatusSensor == null) return false;

            try
            {
                // Lấy ParkingSpaceId từ ParkingStatusSensor
                var parkingSpace = await _parkingSpaceRepository.GetById(parkingStatusSensor.ParkingSpaceId);

                if (parkingSpace == null) return false;

                // Cập nhật trạng thái của ParkingSpace
                if (parkingSpace.Status == (int)ParkingSpaceStatus.Occupied)
                {
                    parkingSpace.Status = (int)ParkingSpaceStatus.Pending;
                }
                else if(parkingSpace.Status == (int)ParkingSpaceStatus.Pending)
                {
                    parkingSpace.Status = (int)ParkingSpaceStatus.Occupied;
                }

                // Cập nhật ParkingStatusSensor - State
                parkingStatusSensor.IsActive = parkingSpace.Status == (int)ParkingSpaceStatus.Occupied;

                await _parkingSpaceRepository.Update(parkingSpace);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

