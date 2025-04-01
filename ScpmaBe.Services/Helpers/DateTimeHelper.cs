namespace ScpmaBe.Services.Helpers
{
    public static class DateTimeHelper
    {
        public static DateOnly ToDateOnly(this DateTime value)
        {
            return new DateOnly(value.Year, value.Month, value.Day);
        }

        public static DateTime ToVNTime(this DateOnly value)
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vnTime = TimeZoneInfo.ConvertTime(value.ToDateTime(TimeOnly.MinValue), vnTimeZone);
            return vnTime;
        }

        public static DateTime ToVNTime(this DateTime value)
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vnTime = TimeZoneInfo.ConvertTime(value, vnTimeZone);
            return vnTime;
        }
    }
}
