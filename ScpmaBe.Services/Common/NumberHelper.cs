namespace ScpmaBe.Services.Common
{
    public static class NumberHelper
    {
        public static string FormatNumberWithVietnameseFormat(int number)
        {
            return number.ToString("#,##0", new System.Globalization.CultureInfo("vi-VN"));
        }

        public static string FormatNumberWithVietnameseFormat(decimal number)
        {
            return number.ToString("#,##0", new System.Globalization.CultureInfo("vi-VN"));
        }
    }
}
