using System.Linq;
using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    /// <summary>
    ///     Tests for CSV models
    /// </summary>
    [TestClass]
    public class CsvModelTests
    {
        private readonly MockFileSystem _files = new();

        private void Test(string csv, string template, string expected)
        {
            new ApplicationEngine(new RunTimeEnvironment(_files))
                .WithTemplate(template)
                .WithModel("model", csv, ModelFormat.Csv)
                .Render()
                .Output
                .Should()
                .Be(expected);
        }

        [TestMethod]
        public void SimpleCsvRead()
        {
            Test(
                @"Code,Text
100,a description",
                "{{model[0].Code}}",
                "100");
        }

        [TestMethod]
        public void CsvInterpretsNumbers()
        {
            Test(@"Code
100",
                "{{model[0].Code * 2}}",
                "200");
        }

        [TestMethod]
        public void CsvInterpretsFloats()
        {
            Test(@"Code
1.5",
                "{{model[0].Code * 2}}",
                "3");
        }

        [TestMethod]
        public void CsvInterpretsBool()
        {
            Test(@"T,F
true,false",
                "{{model[0].T || model[0].F}}",
                "true");
        }


        [TestMethod]
        public void EmptyColumnsAreOk()
        {
            Test(@"Empty,F
,false",
                "-{{model[0].Empty}}-{{model[0].F}}",
                "--false");
        }

        [TestMethod]
        public void CanSerialiseCsvFromWithinScriban()
        {
            var o = Enumerable.Range(1, 2).Select(i => new {id = i, name = $"-{i}-"}).ToArray();
            var template = "{{textrude.to_csv model}}";
            new ApplicationEngine(new RunTimeEnvironment(_files))
                .WithHelpers()
                .WithTemplate(template)
                .WithModel("model", o)
                .RenderToErrorOrOutput()
                .Should().Be(
                    @"id,name
1,-1-
2,-2-
");
        }

        [TestMethod]
        public void CanMutateMemberProperties()
        {
            var csv = @"id
1
";
            var template = @"{{c[0].id=99
c[0].id
}}";
            new ApplicationEngine(new RunTimeEnvironment(_files))
                .WithTemplate(template)
                .WithModel("c", csv, ModelFormat.Csv)
                .RenderToErrorOrOutput()
                .Should()
                .Be(@"99");
        }
    }
}
