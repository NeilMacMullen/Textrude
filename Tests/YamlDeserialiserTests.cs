using System.Collections.Generic;
using Engine.Model;
using Engine.Model.Deserializers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class YamlDeserializerTests
    {
        private Model GetModel(object obj)
        {
            var ser = new YamlModelDeserializer();
            var text = ser.Serialize(obj);

            return new YamlModelDeserializer().Deserialize(text);
        }

        private Dictionary<string, object> GetGraph(object obj)
        {
            return GetModel(obj).Untyped as Dictionary<string, object>;
        }

        private object[] GetArray(object obj)
        {
            return GetModel(obj).Untyped as object[];
        }


        [TestMethod]
        public void NumbersAreCorrectlyDeserialised()
        {
            var graph = GetGraph(new
            {
                A = 1
            });
            graph["A"].Should().Be(1);
        }


        [TestMethod]
        public void BooleansAreCorrectlyDeserialised()
        {
            var graph = GetGraph(new
            {
                A = true
            });
            graph["A"].Should().Be(true);
        }

        [TestMethod]
        public void StringsThatLookLikeNumbersAreNotDeserialisedAsNumbers()
        {
            var graph = GetGraph(new
            {
                A = "1"
            });
            graph["A"].Should().Be("1");
        }

        [TestMethod]
        public void StringsThatLookLikeBoolssAreNotDeserialisedAsBools()
        {
            var graph = GetGraph(new
            {
                A = "true"
            });
            graph["A"].Should().Be("true");
        }


        [TestMethod]
        public void SequenceCanBeDeserialised()
        {
            var graph = GetArray(new[] {"cat", "dog"});
            graph.Should().BeEquivalentTo(new[] {"cat", "dog"});
        }
    }
}