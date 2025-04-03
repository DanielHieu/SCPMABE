namespace ScpmaBe.Services.Models
{
    public class AddContractRequest
    {
        public int CarId { get; set; }

        public int ParkingLotId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Note { get; set; }
    }
}
