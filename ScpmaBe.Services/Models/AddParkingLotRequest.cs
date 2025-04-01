namespace ScpmaBe.Services.Models
{
    public  class AddParkingLotRequest
    {
        public string ParkingLotName { get; set; }
        public decimal PricePerHour { get; set; }

        public decimal PricePerDay { get; set; }

        public decimal PricePerMonth { get; set; }

        public string Address { get; set; }

        public double? Long { get; set; }

        public double? Lat { get; set; }
    }
}
