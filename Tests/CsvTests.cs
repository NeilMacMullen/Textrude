using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CsvTests
    {
        private readonly MockFileSystem _files = new();

        private void Test(string csv, string template, string expected)
        {
            new ApplicationEngine(new RunTimeEnvironment(_files))
                .WithTemplate(template)
                .WithModel(csv, ModelFormat.Csv)
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
    }
}