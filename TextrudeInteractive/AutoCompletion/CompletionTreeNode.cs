using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Engine.Application;

namespace TextrudeInteractive.AutoCompletion
{
    /// <summary>
    ///     Simple tree structure to allow us to suggest auto completions
    /// </summary>
    /// <remarks>
    ///     We don't current support back-tracking/partial matching and we certainly don't make use
    ///     of any parser data so won't always manage to make the most relevant suggestions.  The
    ///     main missing case here is that we don't recognise arrays or variable substitutions so
    ///     can't provide very good matches for parts of models.
    /// </remarks>
    public class CompletionTreeNode
    {
        public static readonly CompletionTreeNode Empty =
            new(string.Empty, Array.Empty<CompletionTreeNode>());

        public readonly ImmutableArray<CompletionTreeNode> Children;
        public readonly string Stem;


        public CompletionTreeNode(string stem, IEnumerable<CompletionTreeNode> children)
        {
            Stem = stem;
            Children = children.ToImmutableArray();
        }

        public CompletionTreeNode Find(ModelPath tokens)
        {
            if (tokens.IsEmpty) return Empty;
            if (tokens.RootToken != Stem) return Empty;

            if (!tokens.IsParent) return this;

            foreach (var child in Children)
            {
                var c = child.Find(tokens.Child());
                if (c != Empty)
                    return c;
            }

            return Empty;
        }

        public CompletionTreeNode Find(string path) => Find(ModelPath.FromString(path));

        public static ImmutableArray<CompletionTreeNode> Build(IEnumerable<ModelPath> paths)
        {
            var nodes = paths
                .Where(p => !p.IsEmpty)
                .GroupBy(p => p.RootToken)
                .ToArray();
            return nodes.Select(n =>
                new CompletionTreeNode(n.Key, Build(n.Select(p => p.Child())))
            ).ToImmutableArray();
        }
    }
}