using System;
using System.Collections.Generic;
using System.Linq;

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
        if (o is not IEnumerable<object> arr)
            return string.Empty;
        return string.Join(Environment.NewLine, arr.Select(a => a.ToString().TrimEnd()));
    }
}
