using System;
using System.Linq;
using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TextrudeExtensionMethodTests
    {
        private readonly MockFileSystem _files = new();
        private readonly RunTimeEnvironment _rte;


        public TextrudeExtensionMethodTests() => _rte = new RunTimeEnvironment(_files);

        [TestMethod]
        public void FunctionsCanBeMovedToLibrary()
        {
            var template =
                @"{{
func __test_f1(x)
x
end
textrude.create_library this  ""test""
test.f1 ""abc""
-}}
";

            var e = new ApplicationEngine(_rte)
                .WithTemplate(template)
                .WithHelpers()
                .Render();
            e.Errors.Should().BeEmpty();
            e.Output
                .Should()
                .Be("abc");
        }
    }


    [TestClass]
    public class ApplicationEngineTests
    {
        private readonly MockFileSystem _files = new();
        private readonly RunTimeEnvironment _rte;


        public ApplicationEngineTests() => _rte = new RunTimeEnvironment(_files);

        [TestMethod]
        public void CodeCompletionShowsModel()
        {
            var model =
                @"str: a";

            new ApplicationEngine(_rte)
                .WithModel(model, ModelFormat.Yaml)
                .ModelPaths()
                .Select(p => p.Render())
                .Should()
                .Contain("model.str");
        }

        [TestMethod]
        public void CodeCompletionShowsDefinitions()
        {
            new ApplicationEngine(_rte)
                .WithDefinitions(new[] {"abc=def"})
                .ModelPaths()
                .Select(p => p.Render())
                .Should()
                .Contain("def.abc");
        }

        [TestMethod]
        public void CodeCompletionShowsEnvironment()
        {
            var envKeys = Environment.GetEnvironmentVariables()
                .Keys
                .Cast<string>()
                .Select(e => $"env.{e}");

            new ApplicationEngine(_rte)
                .WithEnvironmentVariables()
                .ModelPaths()
                .Select(p => p.Render())
                .Should().Contain(envKeys);
        }

        [TestMethod]
        public void CodeCompletionRejectsLibraryMethods()
        {
            var offeredPaths = new ApplicationEngine(_rte)
                .WithTemplate(@"
{{
func __library ; ret 1;end;
func notlibrary ; ret 1;end;
end
}}")
                .Render()
                .ModelPaths()
                .Select(p => p.Render())
                .ToArray();

            offeredPaths
                .Should()
                .Contain("notlibrary");

            offeredPaths
                .Should()
                .NotContain("__library");
        }
    }
}
