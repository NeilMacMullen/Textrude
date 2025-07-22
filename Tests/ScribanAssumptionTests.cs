using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using Tests.Helpers;

namespace Tests;

/// <remarks>
///     These tests track issues/behaviour with the SCRIBAN engine
/// </remarks>
/// >
[TestClass]
public class ScribanAssumptionTests
{
    /// <summary>
    ///     Deliberately ignored
    /// </summary>
    /// <remarks>
    ///     This appears to be deliberate behaviour - see https://github.com/scriban/scriban/issues/297
    /// </remarks>
    [Ignore("Deliberate Scriban behaviour")]
    [TestMethod]
    public void OutputIsAccessibleAfterCapture()
    {
        var text = @"outer1{{capture test}}text{{end}}outer2";
        var compiledTemplate = Template.Parse(text);
        var scriptObject1 = new ScriptObject();
        var context = new TemplateContext();
        context.PushGlobal(scriptObject1);
        var result = compiledTemplate.Render(context);
        result.Should().Be("outer1outer2");
        context.Output.ToString().Should().Be("outer1outer2");
    }

    /// <summary>
    ///     Regression test for https://github.com/scriban/scriban/issues/298
    /// </summary>
    [TestMethod]
    public void IncludeShouldWorkWhenStrictVariablesUsed()
    {
        var text = @"{{include 'testfile'}}";
        var context = new TemplateContext { TemplateLoader = new MockTemplateLoader() };
        //NOTE - setting strict variables previously caused the test to fail
        context.StrictVariables = true;
        var compiledTemplate = Template.Parse(text);
        context.PushGlobal(new ScriptObject());

        var result = compiledTemplate.Render(context);
        result.Should().Be("some text");
    }

    [TestMethod]
    public void ShouldNotBlowUpWithMalformedInput()
    {
        var text = @"{{T ""m"" b:";
        var context = new TemplateContext();
        context.PushGlobal(new ScriptObject());
        Action act = () => Template.Parse(text);
        act.Should().NotThrow();
    }

    [TestMethod]
    public void AdditionWorksWithNumbers()
    {
        var text = @"{{a+b}}";
        var compiledTemplate = Template.Parse(text);
        var scriptObject1 = new ScriptObject();
        scriptObject1.Add("a", 1);
        scriptObject1.Add("b", 2);

        var context = new TemplateContext();
        context.PushGlobal(scriptObject1);

        var result = compiledTemplate.Render(context);
        compiledTemplate.HasErrors.Should().BeFalse();
        result.Should().Be("3");
    }

    [TestMethod]
    public void DictionaryStringStringCanBeUsedInsteadOfScriptObject()
    {
        var text = @"{{model.test}}";
        var compiledTemplate = Template.Parse(text);
        var scriptObject1 = new ScriptObject();
        var dict = new Dictionary<string, string> { ["test"] = "123" };
        scriptObject1.Add("model", dict);
        var context = new TemplateContext();
        context.PushGlobal(scriptObject1);

        var result = compiledTemplate.Render(context);
        compiledTemplate.HasErrors.Should().BeFalse();
        result.Should().Be("123");
    }

    [TestMethod]
    public void BuiltInsCanBeLocated()
    {
        var context = new TemplateContext();


        string[] funcNames(ScriptObject o, string prefix) => o.Where(kv => kv.Value is IScriptFunctionInfo)
            .Select(kv => $"{prefix}.{kv.Key}").ToArray();

        var builtins = context
            .BuiltinObject
            .Select(kv => new { Name = kv.Key, FunctionList = kv.Value as ScriptObject })
            .Where(o => o.FunctionList != null)
            .SelectMany(ns => funcNames(ns.FunctionList, ns.Name))
            .ToArray();

        builtins.Should().Contain("string.contains");
        builtins.Should().Contain("date.now");
    }


    /// <summary>
    ///     Tracks https://github.com/scriban/scriban/issues/315
    /// </summary>
    [TestMethod]
    public void ArrayEachExistsAndWorksAsExpected()
    {
        var script = @"
{{ [""  a  "", ""  b  "", ""  c   ""] | array.each @string.strip }}
";
        var template = Template.Parse(script);
        var result = string.Empty;
        Action render = () => result = template.Render();
        render.Should().NotThrow();
        result.Should().NotContain("not found", "because function should exist");
        result.Trim().Should().Be(@"[""a"", ""b"", ""c""]");
    }


    [TestMethod]
    public void CurryingWorks()
    {
        var script = @"{{func a(lst,x)
for i in lst;i+x;end
end
[1,2,3] | a 5 -}}
";
        var template = Template.Parse(script);
        var result = template.Render();
        result.Should().Be("678");
    }


    [TestMethod]
    public void PreventsInfiniteRecursionOnSelfAssignment()
    {
        var script = @"{{

m = {
   x: 123,
   def: 456 
 }
m.abc =m
m
}}
";

        var a = () =>
        {
            var context = new TemplateContext
            {
                ObjectRecursionLimit = 100
            };
            Template
                .Parse(script)
                .Render(context);
        };
        a.Should().Throw<ScriptRuntimeException>();
    }


    [TestMethod]
    public void PreventsInfiniteRecursionOnSelfAssignmentInLoop()
    {
        var script = @"{{

m = {
   x: 123,
   def: 456 
 }
m.abc =m
m
}}
";

        var a = () =>
        {
            var context = new TemplateContext
            {
                ObjectRecursionLimit = 100
            };
            Template
                .Parse(script)
                .Render(context);
        };
        a.Should().Throw<ScriptRuntimeException>();
    }
}
