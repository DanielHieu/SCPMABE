using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Enums;

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
        public string ParkingLotName { get; set; }
        public string ParkingLotAddress { get; set; }
        
        public double Lat { get; set; }
        public double Long { get; set; }

        public string Note { get; set; }

        public CarResponse Car { get; set; }
        
        public PaymentContractResponse? PaymentContract { get; set; }

        public string CurrentStatus
        {
            get
            {
                if(Status == ContractStatus.Active.ToString() && (PaymentContract == null || PaymentContract .Status == PaymentContractStatus.Completed.ToString())) 
                    return ContractMixedStatus.Active.ToString();

                if (Status == ContractStatus.Expired.ToString()) 
                    return ContractMixedStatus.Expired.ToString();

                if (PaymentContract != null)
                {
                    return PaymentContract.Status;
                }

                return ContractMixedStatus.Active.ToString();
            }
        }
    }
}
