namespace ScpmaBe.Services.Models
{
    public class UpdateParkingStatusSensorRequest
    {
        public int ParkingStatusSensorId { get; set; }
        public string ApiKey { get; set; }
        public int ParkingSpaceId { get; set; }
    }
}
