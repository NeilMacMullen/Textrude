using System;
using System.Linq;
using Engine.Application;
using Engine.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests;

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
            .WithModel("model", model, ModelFormat.Yaml)
            .ModelPaths()
            .Select(p => p.Render())
            .Should()
            .Contain("model.str");
    }

    [TestMethod]
    public void CodeCompletionShowsDefinitions()
    {
        new ApplicationEngine(_rte)
            .WithDefinitions(new[] { "abc=def" })
            .ModelPaths()
            .Select(p => p.Render())
            .Should()
            .Contain("def.abc");
    }

    [TestMethod]
    public void CodeCompletionShowsEnvironment()
    {
        //Not sure why we need to exclude __COMPAT_LAYER here - possibly
        //injected by VS
        var envKeys = Environment.GetEnvironmentVariables()
            .Keys
            .Cast<string>()
            .Where(s => s != "__COMPAT_LAYER")
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


    [TestMethod]
    public void InfiniteRecursionAvoided()
    {
        var script = @"{{

m = {
   x: 123,
   def: 456 
 }
m.abc =m
m
}}
";

        var res = new ApplicationEngine(_rte)
            .WithTemplate(script)
            .Render();
        res.HasErrors.Should().BeTrue();
    }
}
