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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class AreaResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RentalType { get;set; }
    }

    public class FloorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AreaId { get; set; }
    }

    public class ParkingSpaceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FloorId { get; set; }
        public string Status { get; set; }
    }
}
