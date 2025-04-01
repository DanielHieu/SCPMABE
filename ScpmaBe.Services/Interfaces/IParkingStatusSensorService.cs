using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IParkingStatusSensorService
    {
        Task<List<ParkingStatusSensorResponse>> GetAll();
        Task<bool> AddSensor(AddParkingStausSensorRequest request);
        Task<bool> UpdateSensor(UpdateParkingStatusSensorRequest request);   
    }
}
