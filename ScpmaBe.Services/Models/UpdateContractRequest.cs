using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public class UpdateContractRequest
    {
        public int ContractId { get; set; }

        public int CarId { get; set; }

        public int ParkingSpaceId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Note { get; set; }
    }
}
