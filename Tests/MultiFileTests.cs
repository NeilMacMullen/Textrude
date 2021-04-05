using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MultiFileTests
    {
        private readonly MockFileSystem _files = new();

        [TestMethod]
        public void MultipleModelsCanBeSupplied()
        {
            var csv = @"C
1";
            var json = @"{""a"":2}";
            var template = @"{{csv[0].C + json.a}}";
            new ApplicationEngine(new RunTimeEnvironment(_files))
                .WithTemplate(template)
                .WithModel("csv", csv, ModelFormat.Csv)
                .WithModel("json", json, ModelFormat.Json)
                .Render()
                .ErrorOrOutput
                .Should().Be("3");
        }

        [TestMethod]
        public void MultipleFilesCanBeGenerated()
        {
            var template = @"test1
{{-capture output1}}test2{{end-}}
{{-capture output2}}test3{{end-}}
test4";
            var engine = new ApplicationEngine(new RunTimeEnvironment(_files))
                .WithTemplate(template)
                .Render();
            var res = engine.Output;
            //res.Should().Be("aaa");
            var o = engine.GetOutput(3);
            o[2].Should().Be("test3");
            o[1].Should().Be("test2");
            o[0].Should().Be("test1test4");
        }
    }
}
