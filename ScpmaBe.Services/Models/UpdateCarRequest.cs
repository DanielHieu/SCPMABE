using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public class UpdateCarRequest
    {
        public int CarId { get; set; }  

        public int CustomerId { get; set; }

        public string Model { get; set; }

        public string Color { get; set; }

        public string LicensePlate { get; set; }

        public DateTime RegistedDate { get; set; }

        public bool Status { get; set; }
    }
}
