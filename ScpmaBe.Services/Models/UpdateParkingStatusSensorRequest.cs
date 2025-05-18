namespace ScpmaBe.Services.Models
{
    public class UpdateParkingStatusSensorRequest
    {
        public int ParkingStatusSensorId { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
    }
}
