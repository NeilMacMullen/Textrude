using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ExtensionTests
    {
        private readonly MockFileSystem _files = new();

        private string Render(string template) =>
            new ApplicationEngine(_files)
                .WithTemplate(template)
                .WithHelpers()
                .Render()
                .Output;


        [TestMethod]
        public void HumanizrExtensionAreLoaded()
        {
            Render(
                @"{{""test string"" | humanizr.pascalize}}"
            ).Should().Be("TestString");
        }


        [TestMethod]
        public void DebugExtensionAreLoaded()
        {
            Render(
                    @"{{debug.dump 4}}"
                )
                .Trim()
                .Should().Be("4");
        }
    }
}