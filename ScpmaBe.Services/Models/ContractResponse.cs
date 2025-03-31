using ScpmaBe.Services.Enums;

namespace ScpmaBe.Services.Models
{
    public class ContractResponse
    {
        public int Id { get; set; } 
        public string LicensePlate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public int ParkingSpaceId { get; set; }
    }
}
