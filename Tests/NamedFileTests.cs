using Engine.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedApplication;

namespace Tests
{
    [TestClass]
    public class NamedFileTests
    {
        [TestMethod]
        public void SimplePathFillsInModelAndFormat()
        {
            var n = NamedFileFactory.SplitAssignment("abc", "model");
            n.Name.Should().Be("model");
            n.Path.Should().Be("abc");
            n.Format.Should().Be(ModelFormat.Unknown);
        }

        [TestMethod]
        public void UrlCanBeRecognised()
        {
            var n = NamedFileFactory.SplitAssignment("m=http://test", "model");
            n.Name.Should().Be("m");
            n.Path.Should().Be("http://test");
        }

        [TestMethod]
        public void BareUrlCanBeRecognised()
        {
            var n = NamedFileFactory.SplitAssignment("http://test", "model");
            n.Name.Should().Be("model");
            n.Path.Should().Be("http://test");
        }

        [TestMethod]
        public void StdInCanBeRecognised()
        {
            var n = NamedFileFactory.SplitAssignment("m=-", "model");
            n.Name.Should().Be("m");
            n.Path.Should().Be("-");
        }

        [TestMethod]
        public void FormatCanBeRecognised()
        {
            var n = NamedFileFactory.SplitAssignment("json!m=-", "model");
            n.Name.Should().Be("m");
            n.Path.Should().Be("-");
            n.Format.Should().Be(ModelFormat.Json);
        }

        [TestMethod]
        public void FormatCanBeSpecifiedWithoutModelName()
        {
            var n = NamedFileFactory.SplitAssignment("json!blah", "model");
            n.Name.Should().Be("model");
            n.Path.Should().Be("blah");
            n.Format.Should().Be(ModelFormat.Json);
        }


        [TestMethod]
        public void EmptyStringDoesntCrash()
        {
            var n = NamedFileFactory.SplitAssignment(string.Empty, "model");
            n.Name.Should().Be("model");
            n.Path.Should().Be(string.Empty);
            n.Format.Should().Be(ModelFormat.Unknown);
        }


        [TestMethod]
        public void YamlIsRecognised()
        {
            var n = NamedFileFactory.SplitAssignment("yaml!d:/temp/test.model", "model");
            n.Name.Should().Be("model");
            n.Path.Should().Be("d:/temp/test.model");
            n.Format.Should().Be(ModelFormat.Yaml);
        }
    }
}
