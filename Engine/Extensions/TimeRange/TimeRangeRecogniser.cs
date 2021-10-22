using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Constants = Microsoft.Recognizers.Text.DateTime.Constants;

namespace Engine.Extensions.TimeRange
{
    public static class TimeRangeRecogniser
    {
        private static string[] GetValueFor(ModelResult result, string property)
        {
            if (result.Resolution == null)
                return Array.Empty<string>();
            return ((IList<Dictionary<string, string>>)result.Resolution["values"])
                .SelectMany(v =>
                    v.TryGetValue(property, out var p)
                        ? new[] { p }
                        : Array.Empty<string>()
                )
                .ToArray();
        }


        private static string GetValueFor(ModelResult result) =>
            GetValueFor(result, "value")
                .First();


        public static object Recognise(string input)
        {
            var range = Recognise(input, DateTime.UtcNow);
            return range;
        }


        private static void Dump(ModelResult i)
        {
            Console.WriteLine($"RESULT: {i.TypeName} '{i.Text}'");
            if (i.Resolution == null)
            {
                Console.WriteLine("No resolution");
                return;
            }

            foreach (var c in i.Resolution)
            {
                Console.WriteLine($" {c.Key}");

                if (c.Value is IList<Dictionary<string, string>> lst)
                    foreach (var r in lst)
                    {
                        Console.WriteLine("-----");
                        foreach (var row in r)
                            Console.WriteLine($" {row.Key}  {row.Value}");
                    }
                else Console.WriteLine($"Can't handle {c.Value.GetType().Name}");
            }
        }

        public static TimeSpan[] GetOffsets(ModelResult res)
        {
            return GetValueFor(res, "utcOffsetMins")
                .Select(v => TimeSpan.FromMinutes(int.Parse(v)))
                .ToArray();
        }

        public static DateTime GetUtcTimeFor(string propName, ModelResult res, TimeSpan[] suppliedOffsets,
            TimeZoneInfo local, Func<string, DateTime> parse)
        {
            var t = GetValueFor(res, propName);
            if (!t.Any())
                return DateTime.MinValue;
            var v = parse(t.First());
            v = DateTime.SpecifyKind(v, DateTimeKind.Utc);
            var offsets = GetOffsets(res).Concat(suppliedOffsets).ToArray();
            if (offsets.Any())
                return v - offsets.First();
            v = DateTime.SpecifyKind(v, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(v, local);
        }


        public static DateTime GetUtcTimeFor(string propName, ModelResult res, TimeSpan[] suppliedOffsets,
            TimeZoneInfo local) =>
            GetUtcTimeFor(propName, res, suppliedOffsets, local, DateTime.Parse);

        public static DateTime GetUtcTimeForBareTime(string propName, ModelResult res, TimeSpan[] suppliedOffsets,
            TimeZoneInfo local, DateTime basis)
        {
            DateTime Parse(string t) => DateTime.Parse(basis.ToString("yyyy/MMM/dd") + " " + t.Substring(0, 5));
            return GetUtcTimeFor(propName, res, suppliedOffsets, local, Parse);
        }

        public static object Recognise(string input, DateTime basis)
            => Recognise(input, basis, TimeZoneInfo.Local);

        public static object Recognise(string input, DateTime basis, TimeZoneInfo localTimeZone)
        {
            var res = DateTimeRecognizer.RecognizeDateTime(input, Culture.English,
                DateTimeOptions.EnablePreview, basis);
#if DEBUG_TIMERANGE
            foreach (var r in res)
                Dump(r);
#endif
            bool IsType(ModelResult result, string type) => result.TypeName.Split('.')[1] == type;

            var offsets = res.SelectMany(GetOffsets);

            var tz = offsets.ToArray();

            var durations = res.Where(r => IsType(r, Constants.SYS_DATETIME_DURATION))
                .Select(r => TimeSpan.FromSeconds(double.Parse(GetValueFor(r))))
                .ToArray();

            var startDates = res.Where(r =>
                    IsType(r, Constants.SYS_DATETIME_DATE) || IsType(r, Constants.SYS_DATETIME_DATETIME))
                .Select(r => GetUtcTimeFor("value", r, tz, localTimeZone))
                .ToArray();

            var startDatesFromRanges = res.Where(r =>
                    IsType(r, Constants.SYS_DATETIME_DATEPERIOD) || IsType(r, Constants.SYS_DATETIME_DATETIMEPERIOD))
                .Select(r => GetUtcTimeFor("start", r, tz, localTimeZone))
                .ToArray();

            //if we just have a time it is ambiguous and we need to place it before the basis
            var rawTimes = res.Where(r =>
                    IsType(r, Constants.SYS_DATETIME_TIME))
                //   .SelectMany(r => GetValueFor(r, "value"))
                //
                .Select(r => GetUtcTimeForBareTime("value", r, tz, localTimeZone, basis))
                .ToArray();

            //If there are two times then it's because the recogniser isn't sure if we're using the 24 hour clock or not, in which
            //case take the earliest.
            var times = rawTimes.OrderBy(t => t)
                .Take(1)
                //   .Select(t => ParseUniversal(basis.ToString("yyyy/MMM/dd") + " " + t.Substring(0, 5)))
                .Select(d => d > basis ? d - TimeSpan.FromHours(24) : d)
                .ToArray();

            startDates = startDates.Concat(startDatesFromRanges)
                .Concat(times).ToArray();


            var endDates = res.Where(r =>
                    IsType(r, Constants.SYS_DATETIME_DATEPERIOD) || IsType(r, Constants.SYS_DATETIME_DATETIMEPERIOD))
                .Select(r => GetUtcTimeFor("end", r, tz, localTimeZone))
                .ToArray();

            if (startDates.Length == 1 && endDates.Length == 1)
                return new TimeRange(startDates.First(), endDates.First());


            // This is a single date
            if (!durations.Any() && startDates.Length == 1)
            {
                return startDates.Single();
            }

            if (durations.Length == 1 && startDates.Length == 1)
                return new TimeRange(startDates.Single(), durations.Single());


            throw new ArgumentException($"'{input}' couldn't be interpreted as a date or range of time");
        }
    }
}
