using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public class UpdateAreaRequest
    {
        public int AreaId { get; set; }

        public int ParkingLotId { get; set; }

        public string AreaName { get; set; }

        public int TotalFloor { get; set; }

        public int Status { get; set; }

        public int RentalType { get; set; }

    }
}
