using System.Collections.Generic;
using Engine.Model;
using Engine.Model.Deserializers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class YamlDeserializerTests
{
    private static Model GetModel(object obj)
    {
        var ser = new YamlModelDeserializer();
        var text = ser.Serialize(obj);

        return new YamlModelDeserializer().Deserialize(text);
    }

    private static IDictionary<string, object> GetGraph(object obj) =>
        GetModel(obj).Untyped as IDictionary<string, object>;

    private static object[] GetArray(object obj) => GetModel(obj).Untyped as object[];


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
    public void BooleansAreCorrectlyDeserialized()
    {
        var graph = GetGraph(new
        {
            A = true
        });
        graph["A"].Should().Be(true);
    }


    [TestMethod]
    public void SequenceCanBeDeserialized()
    {
        var graph = GetArray(new[] { "cat", "dog" });
        graph.Should().BeEquivalentTo(new[] { "cat", "dog" });
    }


    /// <summary>
    ///     This test is left failing as a reminder
    /// </summary>
    /// <remarks>
    ///     The yaml model builder currently turns fields that look like numbers or bools into
    ///     actual numbers or bools but we would like to break that assumption by treating quoted
    ///     fields as strings.  This test should start passing when that change is made
    /// </remarks>
    [Ignore("Code changes required")]
    [TestMethod]
    public void StringsThatLookLikeNumbersAreNotDeserializedAsNumbers()
    {
        var graph = new YamlModelDeserializer().Deserialize("A: \"1\"").Untyped
            as Dictionary<string, object>;
        ;
        graph["A"].Should().Be("1");
    }

    /// <summary>
    ///     This test is left failing as a reminder
    /// </summary>
    /// <remarks>
    ///     The yaml model builder currently turns fields that look like numbers or bools into
    ///     actual numbers or bools but we would like to break that assumption by treating quoted
    ///     fields as strings.  This test should start passing when that change is made
    /// </remarks>
    [Ignore("Code changes required")]
    [TestMethod]
    public void StringsThatLookLikeBoolsAreNotDeserializedAsBools()
    {
        var graph = new YamlModelDeserializer().Deserialize("A: \"true\"").Untyped
            as Dictionary<string, object>;
        ;
        graph["A"].Should().Be("true");
    }
}
