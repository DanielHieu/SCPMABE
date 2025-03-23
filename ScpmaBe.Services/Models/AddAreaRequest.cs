using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public  class AddAreaRequest
    {
        public int AreaId { get; set; }

        public int ParkingLotId { get; set; }

        public string AreaName { get; set; }

        public int Status { get; set; }

        public int RentalType { get; set; }

    }
}
