using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public class AddTaskEachRequest
    {
        public int OwnerId { get; set; }

        public string Description { get; set; }
    }
}
