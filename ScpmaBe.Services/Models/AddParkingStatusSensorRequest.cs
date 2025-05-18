namespace ScpmaBe.Services.Models
{
    public class AddParkingStatusSensorRequest
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public int ParkingSpaceId { get; set; }
    }
}
