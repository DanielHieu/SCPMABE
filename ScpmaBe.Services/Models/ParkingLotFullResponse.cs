namespace ScpmaBe.Services.Models
{
    public  class ParkingLotFullResponse
    {
        public ParkingLotResponse ParkingLot { get; set; }
        public List<AreaResponse> Areas { get; set; }
        public List<FloorResponse> Floors { get; set; }
        public List<ParkingSpaceResponse> ParkingSpaces { get; set; }
    }

    public class ParkingLotResponse
    {
        public int ParkingLotId { get; set; }
        public string ParkingLotName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal PricePerMonth { get; set; }
    }

    public class AreaResponse
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string RentalType { get;set; }
        public int TotalFloors { get; set; }
    }

    public class FloorResponse
    {
        public int FloorId { get; set; }
        public string FloorName { get; set; }
        public int AreaId { get; set; }
        public int TotalParkingSpaces { get; set; }
    }

    public class ParkingSpaceResponse
    {
        public int ParkingSpaceId { get; set; }
        public string ParkingSpaceName { get; set; }
        public int FloorId { get; set; }
        public string Status { get; set; }
    }
}
