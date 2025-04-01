namespace ScpmaBe.Services.Models
{
    public class CarAndEntranceResponse
    {
        public int CarId { get; set; }
        public string Thumbnail { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }

        public EntranceResponse Entrance { get; set; }

        public int CustomerId { get; set; }
    }
}
