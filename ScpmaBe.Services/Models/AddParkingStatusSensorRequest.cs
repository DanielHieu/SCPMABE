namespace ScpmaBe.Services.Models
{
    public class AddParkingStatusSensorRequest
    {
        public string ApiKey { get; set; }
        public int ParkingSpaceId { get; set; }
        public string Status { get; set; }
    }
}
