using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class DateTimeRecogniserTests : TimeTestMethods
    {
        [TestMethod]
        public void TimesWithSpecificTimeZone()
        {
            //PST turns into PDT during daylight savings
            ExpectDateTime(" 10am PDT, may 3rd 2021", "17:00  3 may 2021", Tz.Utc);
            ExpectDateTime(" 10am, jan 1 2021 PST", "10:00  1 jan 2021", Tz.Pst);
        }

        [TestMethod]
        public void UtcTimes()
        {
            ExpectDateTime("10am, 5 may 2021 UTC", " 10:00 5 may 2021", Tz.Utc);
        }

        [TestMethod]
        public void LocalTimesToUtc()
        {
            ExpectDateTime("jan 1st, 2021", Tz.Pst, "08:00, 1 jan 2021");
            ExpectDateTime("22 may 2021", Tz.Pst, "07:00 22 may 2021");
            ExpectDateTime("5 may 2021", Tz.Uk, "23:00, 4 may 2021");
        }

        [TestMethod]
        public void DateWithoutYear()
        {
            var basis = DateTime.Parse("1 may 2018");
            ExpectDateTime(basis, "1st dec", Tz.Uk, "1 dec 2017");
            ExpectDateTime(basis, "1st feb", Tz.Uk, "1 feb 2018");
        }

        [TestMethod]
        public void DayWithoutAnyContext()
        {
            var basis = DateTime.Parse("1 feb 2018");
            ExpectDateTime(basis, "tuesday", Tz.Uk, "30-jan-2018");
        }

        [TestMethod]
        public void TimeWithoutAnyContext()
        {
            var basis = DateTime.Parse("2 feb 2018");
            ExpectDateTime(basis, "15:00", Tz.Uk, "1-feb-2018 15:00");
        }

        [TestMethod]
        public void ShortTimeWithoutAnyContext()
        {
            var basis = DateTime.Parse("2 feb 2018");
            ExpectDateTime(basis, "1:00", Tz.Uk, "1-feb-2018 1:00");
        }

        [TestMethod]
        public void MidDay()
        {
            var basis = DateTime.Parse("2 feb 2018");
            ExpectDateTime(basis, "midday", Tz.Uk, "1-feb-2018 12:00");
        }

        [TestMethod]
        public void RelativeTimeShouldBeCorrect()
        {
            var basis = DateTime.UtcNow;
            //"yesterday" defaults to the first instant of the day
            var s = ExpectDateTime(basis, "yesterday");
            var duration = (basis - s);
            duration.Should().BeGreaterOrEqualTo(TimeSpan.FromDays(1));
            duration.Should().BeLessOrEqualTo(TimeSpan.FromDays(2));
        }


        [TestMethod]
        public void FromNow()
        {
            var basis = DateTime.Parse("2 feb 2018");
            ExpectDateTime(basis, "3 hours from now", Tz.Uk, "03:00 2 feb 2018");
        }

        [TestMethod]
        public void DaysAgo()
        {
            var basis = DateTime.Parse("1 feb 2020");
            ExpectDateTime(basis, "4 days ago", Tz.Uk, "28 jan 2020");
        }


        [TestMethod]
        public void FromFromTimeInBst()
        {
            var basis = DateTime.Parse("15:00 22 May 2021");
            ExpectDateTime(basis, "11:47", Tz.Uk, "10:47 22 May 2021");
        }

        [TestMethod]
        public void FromFromTimeInUtc()
        {
            var basis = DateTime.Parse("15:00 22 May 2021");
            ExpectDateTime(basis, "11:47 utc", Tz.Uk, "11:47 22 May 2021");
        }


        [TestMethod]
        public void NonsenseInputShouldThrow()
        {
            var basis = DateTime.UtcNow;
            Action a = () => ExpectDateTime(basis, "gibberish");
            a.Should()
                .Throw<ArgumentException>();
        }

        [TestMethod]
        public void BstTest()
        {
            var d = DateTime.Parse("22 may 2021");

            d.Kind.Should().Be(DateTimeKind.Unspecified);
            var tz = Tz.Uk;
            TimeZoneInfo.ConvertTimeToUtc(d, tz).Hour.Should().Be(23);
        }
    }
}
