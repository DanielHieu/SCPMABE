using ScpmaBe.Services.Enums;

namespace ScpmaBe.Services.Models
{
    public  class AddAreaRequest
    {
        public int ParkingLotId { get; set; }

        public string AreaName { get; set; }

        public RentalType RentalType { get; set; }
    }
}
