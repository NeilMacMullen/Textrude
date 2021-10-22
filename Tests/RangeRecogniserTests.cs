using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class RangeRecogniserTests : TimeTestMethods
    {
        [TestMethod]
        public void RangeWithDurationUnderstood()
        {
            var s = ExpectTimeRange("from 1st feb 2000 for 3 days", Tz.Uk);
            s.Start.Should()
                .Be(DateTime.Parse("1-feb-2000"));
            s.Duration.Should()
                .Be(TimeSpan.FromDays(3));
        }

        [TestMethod]
        public void RangeWithDurationButNotYearUnderstood()
        {
            var basis = DateTime.Parse("1 may 2018");
            var s = ExpectTimeRange(basis, "from 1st feb  for 3 days", Tz.Uk);
            s.Start.Should()
                .Be(DateTime.Parse("1-feb-2018"));
            s.Duration.Should()
                .Be(TimeSpan.FromDays(3));
        }


        [TestMethod]
        public void YesterdayRange()
        {
            var basis = DateTime.Parse("1 feb 2018");
            var yesterday = basis.AddDays(-1);
            var s = ExpectTimeRange(basis, "5am to 9pm yesterday", Tz.Uk);
            s.End.Should()
                .Be(yesterday.AddHours(21));

            s.Start.Should()
                .Be(yesterday.AddHours(5));
        }


        [TestMethod]
        public void RangeWithTwoDatesUnderstood()
        {
            var s = ExpectTimeRange("from 1st to 3rd jun 2000", Tz.Uk);
        }

        [TestMethod]
        public void UtcSpecifierOveridesDaylightSaving()
        {
            var s = ExpectTimeRange("2pm 1st jun 2000  until 15:00 utc", Tz.Pst);
            s.Start.Should()
                .Be(DateTime.Parse("1-jun-2000 14:00"));
            s.Duration.Should()
                .Be(TimeSpan.FromHours(1));
        }

        [TestMethod]
        public void LocalTimeZoneUsed()
        {
            var s = ExpectTimeRange("2pm 1st jun 2000  until 15:00 on 2nd Jun 2000", Tz.Uk);
            s.Start.Should()
                .Be(DateTime.Parse("1-jun-2000 13:00"));
            s.Duration.Should()
                .Be(TimeSpan.FromHours(25));
        }

        [TestMethod]
        public void MiddayUnderstood()
        {
            var s = ExpectTimeRange("from midday 1st jun 2000 for 48 hours", Tz.Uk);
            s.Start.Should()
                .Be(DateTime.Parse("1-jun-2000 11:00"));
            s.Duration.Should()
                .Be(TimeSpan.FromDays(2));
        }


        [TestMethod]
        public void FromFromTimeInBst()
        {
            var basis = DateTime.Parse("15:00 22 May 2021");
            var r = ExpectTimeRange(basis, "from 8:40 to 11:45 today", Tz.Uk);
            r.Start.Should().Be(DateTime.Parse("07:40 22 may 2021"));
            r.End.Should().Be(DateTime.Parse("10:45 22 may 2021"));
        }
    }
}
