using System;
using Engine.Application;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

/// <summary>
///     Tests for the DefinitionParser
/// </summary>
[TestClass]
public class DefinitionParserTests
{
    [TestMethod]
    public void WellFormedDefinitionsShouldParse()
    {
        var d = DefinitionParser.CreateDefinitions(new[]
        {
            "ABC=123",
            "DEF=456"
        });
        d["ABC"].Should().Be("123");
        d["DEF"].Should().Be("456");
    }

    [TestMethod]
    public void SingleLetterDefinitionsWork()
    {
        var d = DefinitionParser.CreateDefinitions(new[]
        {
            "A=1"
        });
        d["A"].Should().Be("1");
    }


    [TestMethod]
    public void DefinitionIdsAreTrimmed()
    {
        var d = DefinitionParser.CreateDefinitions(new[]
        {
            "  A   =1"
        });
        d["A"].Should().Be("1");
    }

    [TestMethod]
    public void DefinitionValuesAreNotTrimmed()
    {
        var d = DefinitionParser.CreateDefinitions(new[]
        {
            "ABC=  1  "
        });
        d["ABC"].Should().Be("  1  ");
    }

    [TestMethod]
    public void MissingIdThrows()
    {
        Action a = () => DefinitionParser.CreateDefinitions(new[]
        {
            "=  1  "
        });

        a.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void MissingValueReturnsEmptyString()
    {
        var d = DefinitionParser.CreateDefinitions(new[]
        {
            "ABC="
        });

        d["ABC"].Should().Be(string.Empty);
    }

    [TestMethod]
    public void DuplicateIdThrows()
    {
        Action a = () => DefinitionParser.CreateDefinitions(new[]
        {
            "ABC=123",
            "ABC =456",
        });

        a.Should().Throw<ArgumentException>();
    }
}
