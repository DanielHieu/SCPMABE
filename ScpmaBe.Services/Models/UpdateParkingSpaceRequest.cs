namespace ScpmaBe.Services.Models
{
    public  class UpdateParkingSpaceRequest
    {
        public int ParkingSpaceId { get; set; }
        public string ParkingSpaceName { get; set; }
        public int Status { get; set; }
    }
}
