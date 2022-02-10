using System;
using Engine.Application;
using Engine.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests;

[TestClass]
public class StructuredModelTests
{
    private readonly MockFileSystem _files = new();
    private readonly RunTimeEnvironment _rte;

    private readonly ModelFormat[] _structParsers =
    {
        ModelFormat.Yaml,
        ModelFormat.Json
    };

    public StructuredModelTests() => _rte = new RunTimeEnvironment(_files);


    private void Test(object obj, string template, Action<string> act)
    {
        foreach (var type in _structParsers)
        {
            var text = ModelDeserializerFactory.Serialise(obj, type);
            var result = new ApplicationEngine(_rte)
                .WithTemplate(template)
                .WithModel("model", text, type)
                .Render()
                .Output;
            act(result);
        }
    }

    [TestMethod]
    public void SimpleStringCanBeInterpreted()
    {
        Test(new
            {
                Test = "testval"
            },
            "{{model.Test}}",
            result =>
                result.Should().Be("testval")
        );
    }


    [TestMethod]
    public void SimpleIntCanBeInterpreted()
    {
        Test(new
            {
                Test = "5"
            },
            "{{model.Test}}",
            result =>
                result.Should().Be("5")
        );
    }

    [TestMethod]
    public void NestedObjectCanBeInterpreted()
    {
        Test(
            new
            {
                Test = new { Sub = "sub" }
            },
            "{{model.Test.Sub}}",
            result =>
                result.Should().Be("sub")
        );
    }

    [TestMethod]
    public void ArrayCanBeInterpreted()
    {
        Test(
            new[] { 1, 2, 3 },
            "{{model[0]}}",
            result =>
                result.Should().Be("1")
        );
    }

    [TestMethod]
    public void ArrayOfObjectsCanBeInterpreted()
    {
        Test(new
            {
                Test = new[] { new { Sub = "sub0" } }
            },
            "{{model.Test[0].Sub}}",
            result =>
                result.Should().Be("sub0")
        );
    }

    [TestMethod]
    public void ObjectWithAllTheTypes()
    {
        Test(
            new
            {
                S = "sub0",
                I = 123,
                F = 45.6,
                G = Guid.Empty,
                D = DateTime.UtcNow,
                B = true
            },
            "-",
            result =>
                result.Should().NotBeEmpty()
        );
    }

    [TestMethod]
    public void NullsInJsonCanBeProcessed()
    {
        var model = "{\"label\": null}";
        var engine = new ApplicationEngine(_rte)
            .WithTemplate("{{model.variable}}")
            .WithModel("model", model, ModelFormat.Json)
            .Render();
        engine.HasErrors.Should().BeFalse();
    }

    [TestMethod]
    public void CommentsInJsonCanBeProcessed()
    {
        var model = @"[
1, //a comment
2, //another comment
3 /* a third comment */
]";
        var engine = new ApplicationEngine(_rte)
                .WithTemplate("{{model[0] + model[2] }}")
                .WithModel("model", model, ModelFormat.Json)
                .Render()
            ;
        engine.HasErrors.Should().BeFalse();
        engine.Output.Should().Be("4");
    }


    [TestMethod]
    public void NumericTypesCanBeAdded()
    {
        Test(
            new
            {
                A = 1,
                B = 2
            },
            "{{model.A + model.B}}",
            result =>
                result.Should().Be("3")
        );
    }

    [TestMethod]
    public void BooleanTypesAreEvaluated()
    {
        Test(
            new
            {
                A = false,
                B = true
            },
            "{{model.A || model.B}}",
            result =>
                result.Should().Be("true")
        );
    }


    [TestMethod]
    public void IntegersAreProvidedAsLongs()
    {
        var l = 0x_1abc_def0_1245_5678;
        Test(
            new
            {
                A = l,
            },
            "{{model.A}}",
            result =>
                result.Should().Be(l.ToString())
        );
    }
}
