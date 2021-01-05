using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tests
{
    public record Inner
    {
        //set up a sensible default value rather than use NULL
        public static readonly Inner Default = new();

        public string Name { get; init; } = "default";
    }

    public record Outer
    {
        public Inner InnerProperty { get; init; } = Inner.Default;
    }

    [TestClass]
    public class RecordTests
    {
        [TestMethod]
        public void RunTest()
        {
            var text = @"{""InnerProperty"":
                          {""Name"":""inner""}
    }";

            //Verify that System.Text.Json does not clobber records
            var p = JsonSerializer.Deserialize<Outer>(text);
            p.InnerProperty.Name.Should().Be("inner");
            Inner.Default.Name.Should().Be("default");

            //Verify that JsonConvert does not clobber records
            var r = JsonConvert.DeserializeObject<Outer>(text);
            r.InnerProperty.Name.Should().Be("inner");

            //This assertion fails
            Inner.Default.Name.Should().Be("default",
                "because we should not clobber the record that was the default value");
        }
    }
}