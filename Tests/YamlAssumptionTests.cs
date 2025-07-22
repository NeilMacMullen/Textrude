using System.Collections.Generic;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedApplication;
using YamlDotNet.Serialization;

namespace Tests;

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

        var propertyA = ((Dictionary<object, object>)graph).First();

        propertyA.Key.Should().Be("A");
        propertyA.Value.Should().Be("1");
    }

    [TestMethod]
    public void BooleansAreDeserializedAsStrings()
    {
        var reader = new StringReader("A: true");

        var graph = new Deserializer().Deserialize(reader);

        var propertyA = ((Dictionary<object, object>)graph).First();

        propertyA.Key.Should().Be("A");
        propertyA.Value.Should().Be("true");
    }

    [TestMethod]
    public void SerializingEmptyRenderOptionsShouldNotGenerateFunnyCharacters()
    {
        var text = new Serializer()
            .Serialize(new RenderOptions());

        //leave this test here in the inverted state so we can tell when (if)
        //the YAML serializer gets fixed
#if YAML_FIXED
            text.Should().NotContain("o0");
            text.Should().NotContain("&o");
            text.Should().NotContain("*o");
#else
        //this is the broken behaviour :-(
        text.Should().Contain("o0");
        text.Should().Contain("&o");
        text.Should().Contain("*o");
#endif
    }
}
