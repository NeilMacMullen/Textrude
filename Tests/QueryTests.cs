using System.Linq;
using System.Text.Json;
using Engine.Application;
using Engine.Model;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests;

[TestClass]
public class QueryTests
{
    public static ApplicationEngine Create() => new(new RunTimeEnvironment(new MockFileSystem()));

    [TestMethod]
    public void WithExposesMembers()
    {
        var query = @"{{
obj={abc:123}
with obj
abc
end
}}";
        Create()
            .WithTemplate(query)
            .RenderToErrorOrOutput()
            .Should().Be("123");
    }


    [TestMethod]
    public void WithExposesJsonMembers()
    {
        var query = @"{{
with obj
abc
end
}}";

        Create()
            .WithModel("obj", new { abc = 123 })
            .WithTemplate(query)
            .RenderToErrorOrOutput()
            .Should().Be("123");
    }

    [TestMethod]
    public void MembersCanBeRemovedForSerialisation()
    {
        var query = @"{{
    model.unwanted =null
textrude.to_json model
}}";

        Create()
            .WithHelpers()
            .WithModel("model", new { abc = 123, unwanted = 99 })
            .WithTemplate(query)
            .RenderToErrorOrOutput()
            .Should()
            .Be(@"{
  ""abc"": 123
}");
    }

    [TestMethod]
    public void ItemsAreRecoverable()
    {
        var query = @"{{
        res=items
}}";

        var itemsIn = Enumerable.Range(1, 2).Select(i => new Test { id = i, name = i.ToString() });
        var engine = Create()
            .WithModel("items", itemsIn)
            .WithTemplate(query)
            .Render();

        var o = engine.TryGetVariableAsJsonString("res", out var res);
        o.Should().BeTrue();
        var itemsOut = JsonSerializer.Deserialize<Test[]>(res);
        itemsOut.Should().BeEquivalentTo(itemsIn);
    }


    [TestMethod]
    public void ModelCanBeMutated()
    {
        var query = @"{{
   model.name='hello'
textrude.to_json model
}}";

        Create()
            .WithHelpers()
            .WithModel("model", new { name = "xyz" })
            .WithTemplate(query)
            .RenderToErrorOrOutput()
            .Should()
            .Be(@"{
  ""name"": ""hello""
}");
    }

    [TestMethod]
    public void JsonModelCanBeMutated()
    {
        var query = @"{{
   model.name='hello'
textrude.to_json model
}}";
        var modelText = ModelDeserializerFactory.Fetch(ModelFormat.Json).Serialize(new { name = "xyz" });
        Create()
            .WithHelpers()
            .WithModel("model", modelText, ModelFormat.Json)
            .WithTemplate(query)
            .RenderToErrorOrOutput()
            .Should()
            .Be(@"{
  ""name"": ""hello""
}");
    }

    [TestMethod]
    public void JsonArrayModelCanBeMutated()
    {
        var query = @"{{
func q(i)
i.name ='hello'
ret i
end
model | array.each @q
textrude.to_json model
}}";
        var items = Enumerable.Range(1, 2).Select(i => new Test { id = i, name = i.ToString() }).ToArray();
        var modelText = ModelDeserializerFactory.Fetch(ModelFormat.Json).Serialize(items);
        Create()
            .WithHelpers()
            .WithModel("model", modelText, ModelFormat.Json)
            .WithTemplate(query)
            .RenderToErrorOrOutput()
            .Should()
            .Contain("hello");
    }


    private class Test
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
