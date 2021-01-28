using System;
using System.Collections.Generic;
using System.Linq;

namespace TextrudeInteractive
{
    public record EngineOutputSet
    {
        public static EngineOutputSet Empty =
            new(Array.Empty<OutputPaneModel>());

        public EngineOutputSet(IEnumerable<OutputPaneModel> panes) => Outputs = panes.ToArray();

        //Required for deserialisation
        public EngineOutputSet()
        {
        }

        public OutputPaneModel[] Outputs { get; init; } = Array.Empty<OutputPaneModel>();
    }

    /// <summary>
    ///     Holds text that can be parsed to become a Model
    /// </summary>
    public record OutputPaneModel
    {
        public OutputPaneModel(string format) => Format = format;

        /// <summary>
        ///     Useful default object
        /// </summary>
        public static OutputPaneModel Empty { get; } = new(string.Empty);


        public string Format { get; init; } = string.Empty;
    }
}