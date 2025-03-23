using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public  class AddParkingSpaceRequest
    {
        public int ParkingSpaceId { get; set; }
        public int FloorId { get; set; }
        public string ParkingSpaceName { get; set; }
        public int Status { get; set; }
    }
}
