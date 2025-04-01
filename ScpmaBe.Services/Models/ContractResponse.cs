namespace ScpmaBe.Services.Models
{
    public class ContractResponse
    {
        public int ContractId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public int ParkingSpaceId { get; set; }
        public string ParkingSpaceName { get; set; }
        public int ParkingLotId { get; set; }
        public string ParkingLotName { get; set; }
        public string ParkingLotAddress { get; set; }
        public string AreaName { get; set; }
        public string FloorName { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Note { get; set; }
        public CarResponse Car { get; set; }

        public int PaymentContractId { get; set; }

        public decimal PricePerMonth { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string CreatedDateString { get; set; }

        public decimal TotalAllPayments { get; set; }

        public string StartDateString
        {
            get
            {
                if (StartDate == default)
                    return string.Empty;

                return StartDate.ToString("dd/MM/yyyy");
            }
        }

        public string EndDateString
        {
            get
            {
                if (EndDate == default)
                    return string.Empty;
                return EndDate.ToString("dd/MM/yyyy");
            }
        }

        public bool NeedToProcess { get; set; }
    }
}
