using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public class UpdateAssignedTaskRequest
    {
        public int AssignedTaskId { get; set; }

        public int TaskEachId { get; set; }

        public int StaffId { get; set; }

        public DateOnly ShiftDate { get; set; }

        public TimeOnly ShiftTime { get; set; }

        public string Address { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
