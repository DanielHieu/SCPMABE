﻿
namespace ScpmaBe.Services.Models
{
    public class SearchEntryExitLogRequest
    {
        public string Keyword { get; set; }
        public int ParkingLotId { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
