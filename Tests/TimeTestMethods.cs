using System;
using Engine.Extensions.TimeRange;
using FluentAssertions;
using TimeZoneConverter;

namespace Tests
{
    public abstract class TimeTestMethods
    {
        protected readonly DateTime Basis = DateTime.Parse("1 feb 2018");


        protected static DateTime ExpectDateTime(DateTime basis, string s)
        {
            if (TimeRangeRecogniser.Recognise(s, basis) is DateTime d)
                return d;
            throw new ArgumentException($"Couldn't interpret '{s}' as datetime");
        }


        protected void ExpectDateTime(string s, TimeZoneInfo localTimeZone, string expect)
            => ExpectDateTime(Basis, s, localTimeZone, expect, Tz.Utc);

        protected void ExpectDateTime(string s, string expect, TimeZoneInfo sourceTimeZone)
            => ExpectDateTime(Basis, s, Tz.Unspecified, expect, sourceTimeZone);

        protected static void ExpectDateTime(DateTime basis, string s, TimeZoneInfo localTimeZone, string expect)
            => ExpectDateTime(basis, s, localTimeZone, expect, Tz.Utc);

        protected static void ExpectDateTime(DateTime basis, string s, TimeZoneInfo localTimeZone, string expect,
            TimeZoneInfo sourceTimeZone)
        {
            Console.WriteLine($"******* {s} ******");
            if (!(TimeRangeRecogniser.Recognise(s, basis, localTimeZone) is DateTime d))
                throw new ArgumentException("expected datetime");
            var expected = ToUtc(expect, sourceTimeZone);
            d.Should().Be(expected);
        }

        protected static DateTime ToUtc(string expect, TimeZoneInfo sourceTimeZone)
            => TimeZoneInfo.ConvertTime(DateTime.Parse(expect), sourceTimeZone, TimeZoneInfo.Utc);

        protected TimeRange ExpectTimeRange(string s, TimeZoneInfo localTimeZone)
            => ExpectTimeRange(Basis, s, localTimeZone);

        protected static TimeRange ExpectTimeRange(DateTime basis, string s, TimeZoneInfo localTimeZone)
        {
            if (TimeRangeRecogniser.Recognise(s, basis, localTimeZone) is TimeRange d)
                return d;
            throw new ArgumentException($"Couldn't interpret '{s}' as time-range");
        }

        public static class Tz
        {
            public static readonly TimeZoneInfo Unspecified = TimeZoneInfo.Local;
            public static readonly TimeZoneInfo Utc = TimeZoneInfo.Utc;
            public static readonly TimeZoneInfo Pst = TZConvert.GetTimeZoneInfo("Pacific Standard Time");
            public static readonly TimeZoneInfo Uk = TZConvert.GetTimeZoneInfo("GMT Standard Time");
        }
    }
}
