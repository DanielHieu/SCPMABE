using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models
{
    public class UpdateFeedbackRequest
    {
        public int FeedbackId { get; set; }

        public int CustomerId { get; set; }

        public string Message { get; set; }

        public DateOnly DaySubmitted { get; set; }
    }
}
