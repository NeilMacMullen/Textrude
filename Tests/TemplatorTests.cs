using System;
using Engine.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TemplatorTests
    {
        private readonly ModelFormat[] StructParsers =
        {
            ModelFormat.Yaml,
            ModelFormat.Json
        };


        private void Test(object obj, string template, Action<string> act)
        {
            foreach (var type in StructParsers)
            {
                var text = ModelDeserializerFactory.Serialise(obj, type);
                var result = new ApplicationEngine()
                    .WithTemplate(template)
                    .WithModel(text, type)
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
                    Test = new {Sub = "sub"}
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
                new[] {1, 2, 3},
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
                    Test = new[] {new {Sub = "sub0"}}
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
    }
}