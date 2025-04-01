namespace ScpmaBe.Services.Models
{
    public class EntrancingCarResponse
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string AreaName { get; set; }
        public string FloorName { get; set; }
        public string ParkingSpaceName { get; set; }
        public string RentalType { get; set; }
    }
}
