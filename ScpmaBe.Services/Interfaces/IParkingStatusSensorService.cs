using ScpmaBe.Services.Models;

namespace ScpmaBe.Services.Interfaces
{
    public interface IParkingStatusSensorService
    {
        Task<List<ParkingStatusSensorResponse>> GetAll();
        Task<bool> AddSensor(AddParkingStatusSensorRequest request);
        Task<bool> UpdateSensor(UpdateParkingStatusSensorRequest request);   
        Task<bool> Delete(int id);
    }
}
