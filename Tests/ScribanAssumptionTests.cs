using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace Tests
{
    [TestClass]
    public class ScribanAssumptionTests
    {
        [TestMethod]
        public void Capture()
        {
            var text = @"{{capture test}}text{{end}}";
            var compiledTemplate = Template.Parse(text);
            var scriptObject1 = new ScriptObject();
            var context = new TemplateContext();
            context.PushGlobal(scriptObject1);

            var result = compiledTemplate.Render(context);
            compiledTemplate.HasErrors.Should().BeFalse();
            var t = context.GetValue(new ScriptVariableGlobal("test"));
            t.ToString().Should().Be("text");

            var u = context.GetValue(new ScriptVariableGlobal("undef"));
            u.Should().BeNull();
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