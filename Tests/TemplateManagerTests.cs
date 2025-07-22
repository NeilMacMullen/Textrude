using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Application;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests;

[TestClass]
public class TemplateManagerTests
{
    private readonly MockFileSystem _files = new();

    [TestMethod]
    public void TemplateLoaderShouldBeAbleToSupplyFile()
    {
        var mgr = new TemplateManager(_files);
        _files.WriteAllText("test", "abcde");
        mgr.SetTemplate("{{include 'test'}}");
        var res = mgr.Render();
        res.Should().Be("abcde");
    }


    [TestMethod]
    public void TemplateProcessorIsAppliedToIncludedFile()
    {
        var mgr = new TemplateManager(_files);
        _files.WriteAllText("test",
            @"{{[1,2,3]
|> array.each @{$0*$0}@
}}"
        );

        mgr.SetTemplate("{{include 'test'}}");
        var res = mgr.Render();
        res.Should().Be("[1, 4, 9]");
    }

    [TestMethod]
    public void CodeCompletionCanFetchBuiltInFunctions()
    {
        var mgr = new TemplateManager(_files);
        var f = mgr.GetBuiltIns().Select(p => p.Render()).ToArray();

        f.Should().Contain("string.replace");
        f.Should().Contain("date.now");
    }

    [TestMethod]
    public void CodeCompletionCanFetchObjects()
    {
        var mgr = new TemplateManager(_files);
        mgr.AddVariable("test", new Dictionary<string, object> { ["element"] = 1 });
        var f = mgr.GetObjectTree().Select(p => p.Render()).ToArray();

        f.Should().Contain("test.element");
    }


    [TestMethod]
    public void CodeCompletionDoesNotThrowIfNullInjected()
    {
        var mgr = new TemplateManager(_files);
        mgr.AddVariable("test", new Dictionary<string, object> { ["element"] = null });
        Action getModelPaths = () => mgr.ModelPaths();
        getModelPaths.Should().NotThrow();
    }


    [TestMethod]
    public void LoopLimitIsLarge()
    {
        var mgr = new TemplateManager(_files);
        var template = @"{{
for a in 1..10000
if (a > 9990)
  "" "" + a + "" ""
end
end
                
            }}";
        mgr.SetTemplate(template);
        var res = mgr.Render();
        res.Should()
            .Contain("9999");
    }
}
