using Engine.Application;
using Engine.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class FormatGuesserTests
{
    [TestMethod]
    public void ObviousCsv()
    {
        var text = @"a,b,c
1,2,3";
        AutoDeserializer.GuessFormat(text).Should().Be(ModelFormat.Csv);
    }

    [TestMethod]
    public void JsonObject()
    {
        var text = @"
{ ";
        AutoDeserializer.GuessFormat(text).Should().Be(ModelFormat.Json);
    }

    [TestMethod]
    public void JsonArray()
    {
        var text = @"
  [ ";
        AutoDeserializer.GuessFormat(text).Should().Be(ModelFormat.Json);
    }

    [TestMethod]
    public void Yaml()
    {
        var text = @" var: 1,2,3";
        AutoDeserializer.GuessFormat(text).Should().Be(ModelFormat.Yaml);
    }

    [TestMethod]
    public void YamlList()
    {
        var text = @" - abc
- def";
        AutoDeserializer.GuessFormat(text).Should().Be(ModelFormat.Yaml);
    }
}
