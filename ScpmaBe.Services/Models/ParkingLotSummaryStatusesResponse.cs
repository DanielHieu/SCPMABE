namespace ScpmaBe.Services.Models
{
    public class ParkingLotSummaryStatusesResponse
    {
        public int ParkingLotId { get; set; }
        public int NumberOfAWalkins { get; set; }
        public int NumberOfUWalkins { get; set; }
        public int NumberOfAContracts { get; set; }
        public int NumberOfUContracts { get; set; }
    }
}
