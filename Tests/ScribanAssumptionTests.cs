using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scriban;
using Scriban.Runtime;

namespace Tests
{
    /// <remarks>
    ///     These tests track issues/behaviour with the SCRIBAN engine
    /// </remarks>
    /// >
    [TestClass]
    public class ScribanAssumptionTests
    {
        [TestMethod]
        public void Capture()
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

        [TestMethod]
        public void Include()
        {
            var text = @"{{include 'testfile'}}";
            var context = new TemplateContext {TemplateLoader = new MockTemplateLoader()};
            //NOTE - setting strict variables causes the test to fail
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
            var dict = new Dictionary<string, string> {["test"] = "123"};
            scriptObject1.Add("model", dict);
            var context = new TemplateContext();
            context.PushGlobal(scriptObject1);

            var result = compiledTemplate.Render(context);
            compiledTemplate.HasErrors.Should().BeFalse();
            result.Should().Be("123");
        }
    }
}