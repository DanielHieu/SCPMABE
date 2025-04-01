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
                                ParkingStatusSensorId = x.ParkingStatusSensorId,
                                Name = $"IR{x.ParkingStatusSensorId:00#}",
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
                ApiKey = request.ApiKey,
                ParkingSpaceId = request.ParkingSpaceId,
                IsActive = request.Status == "Active"
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

            sensor.ApiKey = request.ApiKey;
            sensor.IsActive = request.Status == "Active";

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
    }
}
