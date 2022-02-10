using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests;

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
