using System;

namespace Engine.Extensions.TimeRange
{
    public static class MyRecogniser
    {
        public static DateTime RecogniseFromObject(object o)
        {
            switch (o)
            {
                case null:
                    throw new ArgumentException("Unable to treat NULL as a DateTime");
                case DateTime d:
                    return d;
                case int unixTimeShort:
                    return ((long) unixTimeShort).FromUnixTime();
                case long unixTime:
                    return unixTime.FromUnixTime();
                case string str when long.TryParse(str, out var unixTime2):
                    return unixTime2.FromUnixTime();
                case string str:
                {
                    var val = TimeRangeRecogniser.Recognise(str);
                    if (val is DateTime t)
                        return t;
                    throw new ArgumentException($"Unable to interpret '{str}' as a DateTime");
                }
                default:
                    throw new ArgumentException("Unable to treat argument as DateTime");
            }
        }
    }
}
