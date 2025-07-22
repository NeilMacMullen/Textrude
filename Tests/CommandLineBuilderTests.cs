using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedApplication;

namespace Tests;

[TestClass]
public class CommandLineBuilderTests
{
    [TestMethod]
    public void EmptyOptionsIncludesTemplate()
    {
        var options = new RenderOptions();
        var builder = new CommandLineBuilder(options);
        var cmd = builder.BuildRenderInvocation();
        cmd.Should().Contain("render");
        cmd.Should().Contain("--template");
    }


    [TestMethod]
    public void DefinitionsNotShownIfNonePresent()
    {
        var options = new RenderOptions
        {
            Definitions = new[] { string.Empty }
        };
        var builder = new CommandLineBuilder(options);
        var cmd = builder.BuildRenderInvocation();
        cmd.Should().NotContain("definitions");
    }

    [TestMethod]
    public void IncludesNotShownIfNonePresent()
    {
        var options = new RenderOptions
        {
            Include = new[] { string.Empty }
        };
        var builder = new CommandLineBuilder(options);
        var cmd = builder.BuildRenderInvocation();
        cmd.Should().NotContain("include");
    }

    [TestMethod]
    public void UsesModelNames()
    {
        var options = new RenderOptions
        {
            Models = new[] { "test=filename" }
        };

        var builder = new CommandLineBuilder(options);
        var cmd = builder.BuildRenderInvocation();
        cmd.Should().Contain("--models test=filename");
    }

    [TestMethod]
    public void QuotesFileNamesWithSpaces()
    {
        var options = new RenderOptions
        {
            Models = new[] { "test=file name" }
        };

        var builder = new CommandLineBuilder(options);
        var cmd = builder.BuildRenderInvocation();
        cmd.Should().Contain("--models \"test=file name\"");
    }
}
