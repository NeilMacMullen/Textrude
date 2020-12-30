using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlDotNet.Serialization;

namespace Tests
{
    [TestClass]
    public class YamlAssumptionTests
    {
        [TestMethod]
        public void NumbersShouldBeDeserialized()
        {
            var reader = new StringReader("A: 001");

            var graph = new Deserializer().Deserialize(reader);

            var propertyA = ((Dictionary<object, object>) graph).First();

            propertyA.Key.Should().Be("A"); //passes
            propertyA.Value.Should().Be(1); //fails - the value is actually the string "1"
        }

        [TestMethod]
        public void BooleansShouldBeDeserialized()
        {
            var reader = new StringReader("A: true");

            var graph = new Deserializer().Deserialize(reader);

            var propertyA = ((Dictionary<object, object>) graph).First();

            propertyA.Key.Should().Be("A"); //passes
            propertyA.Value.Should().Be(true); //fails - the value is actually the string "1"
        }
    }
}