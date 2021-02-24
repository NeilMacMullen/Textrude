using System;
using System.Linq;
using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Tests
{
    [TestClass]
    public class VersionTests
    {
        [TestMethod]
        public void VersionComparisonWorks()
        {
            UpgradeManager.CompareVersions("0.0.0", "0.0.0").Should().Be(0);

            UpgradeManager.CompareVersions("1.0.0", "0.0.0").Should().BeGreaterThan(0);
            UpgradeManager.CompareVersions("0.1.0", "0.0.0").Should().BeGreaterThan(0);
            UpgradeManager.CompareVersions("0.0.1", "0.0.0").Should().BeGreaterThan(0);


            UpgradeManager.CompareVersions("6.0.0", "5.5.5").Should().BeGreaterThan(0);
            UpgradeManager.CompareVersions("5.6.0", "5.5.5").Should().BeGreaterThan(0);
            UpgradeManager.CompareVersions("5.5.6", "5.5.5").Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void VersionComparisonTreatsMalformedAsZero()
        {
            UpgradeManager.CompareVersions("0.0.0", "1.1").Should().Be(0);
        }

        [TestMethod]
        public void DeserialisationOfChangeListIsAsExpected()
        {
            var text = @"[
{
  ""Version"" : ""0.2.1"",
            ""Date"" :""5 jan 2020"",
            ""Summary"" : ""test"",
            ""Detail"" : [
            ""line1"",
            ""line2""
                ]
        }
        ]";

            var i = JsonConvert.DeserializeObject<UpgradeManager.VersionInfo[]>(text).Single();
            i.Version.Should().Be("0.2.1");
            i.Date.Should().Be(DateTime.Parse("5 jan 2020"));
            i.Summary.Should().Be("test");
        }
    }
}