using System;
using System.Collections.Generic;
using System.Linq;
using Engine.TemplateProcessing;

namespace Engine.Model.Deserializers;

/// <summary>
///     Deserializes a text file by splitting it into lines
/// </summary>
public class LineModelDeserializer : IModelDeserializer
{
    public Model Deserialize(string input)
    {
        var lines = input.Split(Environment.NewLine);
        return new Model(ModelFormat.Line, lines);
    }

    public string Serialize(object o)
    {
        return o is not IEnumerable<object> arr
            ? string.Empty
            : string.Join(Environment.NewLine, arr.Select(a => a.ToString().EmptyWhenNull().TrimEnd()));
    }
}
