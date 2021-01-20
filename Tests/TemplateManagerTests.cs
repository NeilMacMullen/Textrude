using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
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
    }
}