using System;
using System.Linq;
using Engine.Application;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedApplication;
using Tests.Helpers;
using Textrude;

namespace Tests;

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
        var act = Run;
        act.Should().Throw<ApplicationException>().WithMessage("*missing*nofile*");
    }

    [TestMethod]
    public void ProvidesCorrectFileNameInErrorMessageWhenFileNonExistent()
    {
        _options.Models = _options.Models.Append(Name("model", "elusive.json")).ToArray();
        AddTemplate("{{model.A}} ddd");
        var act = Run;
        act.Should().Throw<ApplicationException>().WithMessage("*elusive.json*");
    }

    [TestMethod]
    public void ProvidesCorrectFileNameInErrorMessageWhenReadFault()
    {
        var problemFile = "unreadable.json";
        AddModel(problemFile, "abcd");
        _fileSystem.ThrowOnRead(problemFile);
        AddTemplate("{{model.A}} ddd");
        var act = Run;
        act.Should().Throw<ApplicationException>().WithMessage($"*{problemFile}*");
    }

    [TestMethod]
    public void SimpleModelGeneratesOutput()
    {
        AddModel(ModelFile, "A: 123");
        AddTemplate("{{model.A}} ddd");
        AddOutputFile(OutputFile);

        Run();
        _fileSystem.ReadAllText(OutputFile)
            .Should().Be("123 ddd");
    }

    [TestMethod]
    public void NamedModelsAreUnderstood()
    {
        AddModel("test", ModelFile, "A: 123");
        AddTemplate("{{test.A}} ddd");
        AddOutputFile(OutputFile);

        Run();
        _fileSystem.ReadAllText(OutputFile)
            .Should().Be("123 ddd");
    }

    [TestMethod]
    public void NamedOutputsAreUnderstood()
    {
        AddModel(ModelFile, "A: 123");
        AddTemplate("{{capture testOut}}hello{{end}} ");
        AddOutputFile("testOut", OutputFile);

        Run();
        _fileSystem.ReadAllText(OutputFile)
            .Should().Be("hello");
    }

    [TestMethod]
    public void DynamicOutputsAreUnderstood()
    {
        AddModel(ModelFile, "A: 123");
        AddTemplate(@"
{{

capture xout
""this is the value of x""
end
textrude.add_output this ""abc.txt"" xout
}}
");

        UseDynamicOutput();
        Run();
        _fileSystem.ReadAllText("abc.txt")
            .Should().Be("this is the value of x");
    }

    [TestMethod]
    public void EmptyDynamicOutputsDoNotCrash()
    {
        AddModel(ModelFile, "A: 123");
        AddTemplate(@"");
        UseDynamicOutput();
        Run();
    }


    [TestMethod]
    public void WhenLazyEngineDoesNotRunUnlessInputTouched()
    {
        _options.Lazy = true;
        AddModel(ModelFile, "A: 123");
        AddTemplate("{{model.A}}");
        AddOutputFile(OutputFile);
        //create this last so that it post-dates the input
        _fileSystem.WriteAllText(OutputFile, string.Empty);

        Run();
        _fileSystem.ReadAllText(OutputFile).Should().BeEmpty();

        _fileSystem.Touch(ModelFile);
        Run();
        _fileSystem.ReadAllText(OutputFile).Should().Be("123");

        _fileSystem.WriteAllText(TemplateFile, "abc");

        Run();
        _fileSystem.ReadAllText(OutputFile).Should().Be("abc");
    }


    [TestMethod]
    public void WhenLazyEngineRunsIfOutputDoesNotExist()
    {
        _options.Lazy = true;
        AddModel(ModelFile, "A: 123");
        AddTemplate("{{model.A}}");
        AddOutputFile(OutputFile);
        Run();
        _fileSystem.ReadAllText(OutputFile).Should().Be("123");
    }

    #region helper methods

    private const string ModelFile = "model.yaml";
    private const string OutputFile = "output.txt";
    private const string TemplateFile = "template.sbn";
    private readonly MockFileSystem _fileSystem = new();
    private readonly MockFileSystem _mockWeb = new();

    private readonly Textrude.Helpers _helper = new()
    {
        ExitHandler =
            msg => throw new ApplicationException(msg)
    };

    private readonly RenderOptions _options = new();


    private void Run()
    {
        var hybrid = new HybridFileSystem(_fileSystem, _mockWeb);
        CmdRender.Run(_options, new RunTimeEnvironment(hybrid), _helper);
    }

    private void UseDynamicOutput()
    {
        _options.DynamicOutput = true;
    }

    private void AddTemplate(string someText)
    {
        _fileSystem.WriteAllText(TemplateFile, someText);
        _options.Template = TemplateFile;
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
