namespace Shared.Helpers;

public static class DateTimeHelper
{
    public static DateTime Local2Utc(DateTime localDate, TimeSpan localTimeOfDay)
    {
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        DateTime localDateTime = localDate.Date + localTimeOfDay;
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);
    }
}
