using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;

namespace ScpmaBe.Services
{
    public class ParkingStatusSensorService : IParkingStatusSensorService
    {
        private readonly IParkingStatusSensorRepository _repository;

        public ParkingStatusSensorService(IParkingStatusSensorRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ParkingStatusSensorResponse>> GetAll()
        {
            return await _repository.GetAll()
                            .Include(x => x.ParkingSpace)
                            .Select(x => new ParkingStatusSensorResponse
                            {
                                Name = x.Name,
                                ParkingStatusSensorId = x.ParkingStatusSensorId,
                                ApiKey = x.ApiKey,
                                ParkingSpaceId = x.ParkingSpaceId,
                                ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                Status = x.IsActive ? "Active" : "Inactive"
                            }).ToListAsync();
        }

        public async Task<bool> AddSensor(AddParkingStatusSensorRequest request)
        {
            var sensor = new ParkingStatusSensor
            {
                Name = request.Name,
                ApiKey = request.ApiKey,
                ParkingSpaceId = request.ParkingSpaceId,
                IsActive = false
            };

            await _repository.Insert(sensor);

            return true;
        }

        public async Task<bool> UpdateSensor(UpdateParkingStatusSensorRequest request)
        {
            var sensor = await _repository.GetById(request.ParkingStatusSensorId);

            if (sensor == null)
            {
                return false;
            }

            sensor.Name = request.Name;
            sensor.ApiKey = request.ApiKey;

            await _repository.Update(sensor);

            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var sensor = await _repository.GetById(id);
            
            if (sensor == null)
            {
                return false;
            }

            await _repository.Delete(id);
            
            return true;
        }

        public async Task<bool> ChangeState(int id, bool isActive)
        {
            var sensor = await _repository.GetById(id);

            if (sensor == null)
            {
                return false;
            }

            sensor.IsActive = isActive;

            await _repository.Update(sensor);

            return true;
        }
    }
}
