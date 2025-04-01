namespace ScpmaBe.Services.Models
{
    public class EntryExitLogResponse
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public string RentalType { get; set; }
        public decimal TotalAmount { get; set; }
        public string EntryTime { get; set; }
        public string ExitTime { get; set; }
        public string ParkingSpaceName { get; set; }
        public string ParkingSpaceStatus { get; set; }
        public bool IsPaid { get; set; }
        public string EntranceImage { get; set; }
        public string ExitImage { get; set; }
    }
}
