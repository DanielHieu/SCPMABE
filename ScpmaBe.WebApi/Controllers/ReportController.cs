using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummaryReport()
        {
            var result = await _reportService.GetSummaryReport();

            return Ok(result);
        }


        [HttpGet("SummaryRevenue")]
        public async Task<IActionResult> GetSummaryRevenueReport()
        {
            var result = await _reportService.GetSummaryRevenueReport();

            return Ok(result);
        }


        [HttpGet("ParkingLotRevenue")]
        public async Task<IActionResult> GetParkingLotRevenueInYearReports()
        {
            var result = await _reportService.GetParkingLotRevenueInYearReports();

            return Ok(result);
        }


        [HttpGet("MonthlyRevenueByTypes")]
        public async Task<IActionResult> GetMonthlyRevenueByTypes(int year)
        {
            var result = await _reportService.GetMonthlyRevenueByTypes(year == 0 ? DateTime.Now.ToVNTime().Year : year);

            return Ok(result);
        }

        [HttpGet("GetWalkinTodayRevenue")]
        public async Task<IActionResult> GetWalkinTodayRevenue()
        {
            var result = await _reportService.GetWalkinDailyRevenue(DateTime.Now.ToVNTime());

            return Ok(result);
        }

        [HttpGet("GetParkingLotContractMonthlyRevenues")]
        public async Task<IActionResult> GetParkingLotContractMonthlyRevenues(int year)
        {
            var result = await _reportService.GetParkingLotContractMonthlyRevenues(year == 0 ? DateTime.Now.ToVNTime().Year : year);
           
            return Ok(result);
        }

        [HttpGet("GetParkingLotWalkinMonthlyRevenues")]
        public async Task<IActionResult> GetParkingLotWalkinMonthlyRevenues(int year)
        {
            var result = await _reportService.GetParkingLotWalkinMonthlyRevenues(year == 0 ? DateTime.Now.ToVNTime().Year : year);

            return Ok(result);
        }
    }
}
