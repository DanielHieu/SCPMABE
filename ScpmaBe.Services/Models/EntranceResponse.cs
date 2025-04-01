namespace ScpmaBe.Services.Models
{
    public class EntranceResponse
    {
        public int EntranceId { get; set; }
        public string EntranceDate { get; set; }
        public string EntranceTime { get; set; }
        public string LicensePlate { get; set; }
        public string FloorName { get; set; }
        public string AreaName { get; set; }
        public string ParkingLotName { get; set; }
        public string ParkingSpaceName { get; set; }
        public string Status { get; set; }
    }
}
