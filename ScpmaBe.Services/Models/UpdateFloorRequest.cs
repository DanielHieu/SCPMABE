using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public class UpdateFloorRequest
    {
        public int FloorId { get; set; }

        public string FloorName { get; set; }

        public int Status { get; set; }
    }
}
