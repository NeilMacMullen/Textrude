using System;
using Engine.TemplateProcessing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class LineProcessingTests
{
    [TestMethod]
    public void EmptyListProcessesToEmptyList()
    {
        var lines = Array.Empty<string>();
        lines
            .ProcessLinePairs(p => throw new ArgumentException()).Should()
            .BeEquivalentTo(Array.Empty<string>());
    }

    [TestMethod]
    public void SingleItemListIsUnaltered()
    {
        var lines = new[] { "a string" };
        lines
            .ProcessLinePairs(p => throw new ArgumentException()).Should()
            .BeEquivalentTo(lines);
    }

    [TestMethod]
    public void ProcessLinePairsCanOperateOnBothFirstAndSecond()
    {
        var lines = new[] { "1", "2", "+abc", "+def" };
        var expected = new[] { "1", "2>", "abc>", "def" };

        LinePair JoinFunc(LinePair p) =>
            p.Second.StartsWith("+")
                ? new LinePair(p.First + ">", p.Second.Substring(1))
                : p;

        lines
            .ProcessLinePairs(JoinFunc)
            .Should()
            .BeEquivalentTo(expected);
    }

    [TestMethod]
    public void PipeSquashing()
    {
        var t = @" a template
    |> pipeline
  |> step2
notpiped";

        var expected = @" a template |>
 pipeline |>
 step2
notpiped";
        var tp = new TemplateProcessor(t);
        tp.HoistPipes();
        tp.Template.Should().Be(expected);
    }


    [TestMethod]
    public void FunctionSnarfing()
    {
        var t = @"st
{{func xyz
abc";

        var expected = @"st
{{-func xyz
abc";
        var tp = new TemplateProcessor(t);
        tp.FunctionSnarf();
        tp.Template.Should().Be(expected);
    }


    [TestMethod]
    public void TerseLambdas()
    {
        var t = @"x | array.each @{$0 *3}@";

        var expected = @"x | array.each @(do;ret ($0 *3);end)";
        var tp = new TemplateProcessor(t);
        tp.TerseLambda();
        tp.Template.Should().Be(expected);
    }


    [TestMethod]
    public void PipeSquashingCanBeDisabledAndReenabled()
    {
        var t = @"
{{# TEXTRUDE push nopipehoist }}
pipeline
|> step2
{{# TEXTRUDE pop nopipehoist }}
pipeline
|> step3
";

        var expected = @"
{{# TEXTRUDE push nopipehoist }}
pipeline
|> step2
{{# TEXTRUDE pop nopipehoist }}
pipeline |>
 step3
";
        var tp = new TemplateProcessor(t);
        tp.HoistPipes();
        tp.Template.Should().Be(expected);
    }

    [TestMethod]
    public void SimpleDirective()
    {
        var d = new PreprocessorDirective("{{# TEXTRUDE PUSH xYz }}");
        d.IsDirective.Should().BeTrue();
        d.Command.Should().Be("push");
        d.Operand.Should().Be("xyz");
    }


    [TestMethod]
    public void DirectiveRecognisedEvenWhenSnuggledAgainstClosingBrace()
    {
        var d = new PreprocessorDirective("{{#TEXTRUDE PUSH xYz}}");
        d.IsDirective.Should().BeTrue();
        d.Command.Should().Be("push");
        d.Operand.Should().Be("xyz");
    }

    [TestMethod]
    public void NotDirective()
    {
        var d = new PreprocessorDirective("{{# EXTRUDE PUSH xYz }}");
        d.IsDirective.Should().BeFalse();
    }


    [TestMethod]
    public void SimpleCommentIsNotDirective()
    {
        var d = new PreprocessorDirective("{{");
        d.IsDirective.Should().BeFalse();
    }


    [TestMethod]
    public void DirectiveDoesntCareAboutWhitespaceControl()
    {
        var d = new PreprocessorDirective("{{ -~ # TexTRUDE PUSH xYz }}");
        d.IsDirective.Should().BeTrue();
    }
}
