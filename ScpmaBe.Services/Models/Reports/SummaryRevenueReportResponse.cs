using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Models.Reports
{
    public class SummaryRevenueReportResponse
    {
        public decimal TotalRevenueInYear { get; set; }
        public decimal ContractRevenueInYear { get; set; }
        public decimal WalkinRevenueInYear { get; set; }

        public decimal TotalRevenueInMonth { get; set; }
        public decimal ContractRevenueInMonth { get; set; }
        public decimal WalkinRevenueInMonth { get; set; }

        public decimal TotalRevenueInWeek { get; set; }
        public decimal ContractRevenueInWeek { get; set; }
        public decimal WalkinRevenueInWeek { get; set; }

        public decimal TotalRevenueInDay { get; set; }
        public decimal ContractRevenueInDay { get; set; }
        public decimal WalkinRevenueInDay { get; set; }

    }
}
