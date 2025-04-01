namespace ScpmaBe.Services.Models
{
    public class ParkingStatusSensorResponse
    {
        public int ParkingStatusSensorId { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public int ParkingSpaceId { get; set; }
        public string ParkingSpaceName { get; set; }
        public string Status { get; set; }
    }
}
