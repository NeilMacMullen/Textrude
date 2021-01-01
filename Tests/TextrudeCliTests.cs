using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Textrude;

namespace Tests
{
    [TestClass]
    public class TextrudeCliTests
    {
        private readonly MockFileSystem _fileSystem = new();

        private readonly Helpers _helper = new()
        {
            ExitHandler =
                msg => throw new ApplicationException(msg)
        };

        private readonly CmdRender.Options _options = new();


        private void Run()
        {
            CmdRender.Run(_options, _fileSystem, _helper);
        }

        [TestMethod]
        public void SimpleRenderPass()
        {
            AddTemplate("some text");
            Run();
        }

        [TestMethod]
        public void ThrowsWhenCantReadTemplate()
        {
            _options.Template = "nofile";
            Action act = () => Run();
            act.Should().Throw<ApplicationException>().WithMessage("*missing*nofile*");
        }

        [TestMethod]
        public void SimpleModelGeneratesOutput()
        {
            AddModel("m.yaml", "A: 123");
            AddTemplate("{{model.A}} ddd");
            AddOutputFile("out.txt");

            Run();
            _fileSystem.ReadAllText("out.txt")
                .Should().Be("123 ddd");
        }

        private void AddTemplate(string someText)
        {
            var fileName = "template.sbn";
            _fileSystem.WriteAllText(fileName, someText);
            _options.Template = fileName;
        }

        private void AddModel(string fileName, string text)
        {
            _fileSystem.WriteAllText(fileName, text);
            _options.Models = _options.Models.Append(fileName).ToArray();
        }

        private void AddOutputFile(string fileName)
        {
            _options.Output = _options.Output.Append(fileName).ToArray();
        }
    }
}