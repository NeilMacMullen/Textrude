using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Tests
{
    /// <summary>
    ///     Tests to confirm behaviour of JsonConvert layer
    /// </summary>
    /// <remarks>
    ///     Strictly speaking these tests are redundant but they serve
    ///     as documentation of the way that JsonConvert performs deserialisation
    /// </remarks>
    [TestClass]
    public class JObjectTests
    {
        [TestMethod]
        public void TestMethod3()
        {
            var json = JObject.FromObject(new {Test = "testval"});
            json.Properties().Count().Should().Be(1);
            var p = json.Properties().First();
            p.Value.Type.Should().Be(JTokenType.String);
            p.Value.Value<string>().Should().Be("testval");
        }


        [TestMethod]
        public void TestMethod11()
        {
            var json = JObject.FromObject(new
            {
                Test = new[]
                {
                    1, 2, 3
                }
            });
            json.Properties().Count().Should().Be(1);
            var p = json.Properties().First();
            p.Name.Should().Be("Test");
            p.Value.Type.Should().Be(JTokenType.Array);
            var elements = p.Value.Children();

            elements.Count().Should().Be(3);
            elements.First()
                .Type.Should().Be(JTokenType.Integer);
            elements.First().Value<int>()
                .Should().Be(1);
        }

        [TestMethod]
        public void TestMethod10()
        {
            var json = JObject.FromObject(new
            {
                Test = new
                {
                    Sub = "subval"
                }
            });
            json.Properties().Count().Should().Be(1);
            var p = json.Properties().First();
            p.Value.Type.Should().Be(JTokenType.Object);
            var sub = p.Value.Value<JObject>();
            sub.Properties().First().Value.Value<string>()
                .Should().Be("subval");
        }
    }
}