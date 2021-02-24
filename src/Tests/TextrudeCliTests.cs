using System;
using System.Linq;
using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedApplication;
using Textrude;

namespace Tests
{
    [TestClass]
    public class TextrudeCliTests
    {
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
            AddModel(modelFile, "A: 123");
            AddTemplate("{{model.A}} ddd");
            AddOutputFile(outputFile);

            Run();
            _fileSystem.ReadAllText(outputFile)
                .Should().Be("123 ddd");
        }

        [TestMethod]
        public void NamedModelsAreUnderstood()
        {
            AddModel("test", modelFile, "A: 123");
            AddTemplate("{{test.A}} ddd");
            AddOutputFile(outputFile);

            Run();
            _fileSystem.ReadAllText(outputFile)
                .Should().Be("123 ddd");
        }

        [TestMethod]
        public void NamedOutputsAreUnderstood()
        {
            AddModel(modelFile, "A: 123");
            AddTemplate("{{capture testOut}}hello{{end}} ");
            AddOutputFile("testOut", outputFile);

            Run();
            _fileSystem.ReadAllText(outputFile)
                .Should().Be("hello");
        }


        [TestMethod]
        public void WhenLazyEngineDoesNotRunUnlessInputTouched()
        {
            _options.Lazy = true;
            AddModel(modelFile, "A: 123");
            AddTemplate("{{model.A}}");
            AddOutputFile(outputFile);
            //create this last so that it post-dates the input
            _fileSystem.WriteAllText(outputFile, string.Empty);

            Run();
            _fileSystem.ReadAllText(outputFile).Should().BeEmpty();

            _fileSystem.Touch(modelFile);
            Run();
            _fileSystem.ReadAllText(outputFile).Should().Be("123");

            _fileSystem.WriteAllText(templateFile, "abc");

            Run();
            _fileSystem.ReadAllText(outputFile).Should().Be("abc");
        }


        [TestMethod]
        public void WhenLazyEngineRunsIfOutputDoesNotExist()
        {
            _options.Lazy = true;
            AddModel(modelFile, "A: 123");
            AddTemplate("{{model.A}}");
            AddOutputFile(outputFile);
            Run();
            _fileSystem.ReadAllText(outputFile).Should().Be("123");
        }

        #region helper methods

        private const string modelFile = "model.yaml";
        private const string outputFile = "output.txt";
        private const string templateFile = "template.sbn";
        private readonly MockFileSystem _fileSystem = new();

        private readonly Helpers _helper = new()
        {
            ExitHandler =
                msg => throw new ApplicationException(msg)
        };

        private readonly RenderOptions _options = new();


        private void Run()
        {
            CmdRender.Run(_options, new RunTimeEnvironment(_fileSystem), _helper);
        }

        private void AddTemplate(string someText)
        {
            _fileSystem.WriteAllText(templateFile, someText);
            _options.Template = templateFile;
        }

        private void AddModel(string fileName, string text)
            => AddModel(string.Empty, fileName, text);

        private string Name(string modelName, string fileName)
            => modelName.Length == 0
                ? fileName
                : $"{modelName}={fileName}";

        private void AddModel(string modelName, string fileName, string text)
        {
            _fileSystem.WriteAllText(fileName, text);
            _options.Models = _options.Models.Append(Name(modelName, fileName)).ToArray();
        }

        private void AddOutputFile(string fileName)
            => AddOutputFile(string.Empty, fileName);

        private void AddOutputFile(string name, string fileName)
        {
            _options.Output = _options.Output.Append(Name(name, fileName)).ToArray();
        }

        #endregion
    }
}
