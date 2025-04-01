namespace ScpmaBe.Services.Models
{
    public class AddEntryExitLogRequest
    {
        public int ParkingSpaceId { get; set; }

        public string LicensePlate { get; set; }

        public int RentalType { get; set; }
        public string EntranceImage { get; set; }
    }
}