using System.Globalization;

namespace Application.Helper;

public static class DateConvertor
{
    public static string ToShamsi(this DateTime value)
    {
        // چک کردن اینکه آیا تاریخ معتبر است یا خیر
        // اگر تاریخ مقداردهی نشده باشد، مقدار آن برابر DateTime.MinValue است
        if (value == DateTime.MinValue)
        {
            return "---"; // یا string.Empty
        }

        try 
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(value) + "/" + 
                   pc.GetMonth(value).ToString("00") + "/" + 
                   pc.GetDayOfMonth(value).ToString("00");
        }
        catch
        {
            // در صورت بروز هرگونه خطای دیگر
            return "نامشخص";
        }
    }
}