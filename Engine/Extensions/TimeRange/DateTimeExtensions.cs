using System;

namespace Engine.Extensions.TimeRange;

public static class DateTimeExtensions
{
    public static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime FromUnixTime(this long unixTime) => UnixEpoch.AddSeconds(unixTime);

    public static long ToUnixTime(this DateTime date) => Convert.ToInt64((date - UnixEpoch).TotalSeconds);

    public static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;


    public static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

    public static DateTime FromUnixTimeString(string sourceTimeSecondsSinceEpoch) =>
        long.Parse(sourceTimeSecondsSinceEpoch)
            .FromUnixTime();
}
