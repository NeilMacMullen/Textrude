using System.Collections.Generic;
using System.Linq;
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
            mgr.AddVariable("test", new Dictionary<string, object> {["element"] = 1});
            var f = mgr.GetObjectTree().Select(p => p.Render()).ToArray();

            f.Should().Contain("test.element");
        }
    }
}