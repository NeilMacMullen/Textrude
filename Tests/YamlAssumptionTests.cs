using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YamlDotNet.Serialization;

namespace Tests
{
    /// <summary>
    /// </summary>
    [TestClass]
    public class YamlAssumptionTests
    {
        [TestMethod]
        public void NumbersAreDeserializedAsStrings()
        {
            var reader = new StringReader("A: 1");

            var graph = new Deserializer().Deserialize(reader);

            var propertyA = ((Dictionary<object, object>) graph).First();

            propertyA.Key.Should().Be("A");
            propertyA.Value.Should().Be("1");
        }

        [TestMethod]
        public void BooleansAreDeserializedAsStrings()
        {
            var reader = new StringReader("A: true");

            var graph = new Deserializer().Deserialize(reader);

            var propertyA = ((Dictionary<object, object>) graph).First();

            propertyA.Key.Should().Be("A");
            propertyA.Value.Should().Be("true");
        }
    }
}