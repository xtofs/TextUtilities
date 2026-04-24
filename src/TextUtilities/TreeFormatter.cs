namespace TextUtilities;



/// <summary>
/// Formats a tree structure into a text representation.
/// </summary>
/// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
/// <remarks>based on Andrew Lock's implementation: https://andrewlock.net/creating-a-tree-view-in-csharp/</remarks>
public readonly record struct TreeFormatter<TNode>(
    Func<TNode, string> GetLabel,
    Func<TNode, IReadOnlyCollection<TNode>> GetChildren)
{

    /// <summary>
    /// Formats a single node and writes the tree representation to the provided writer.
    /// </summary>
    /// <param name="node">The root node to format.</param>
    /// <param name="writer">The text writer to write the formatted output to.</param>
    public void Format(TNode node, TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        Format(node, TreeStyle.Default, writer);
    }

    /// <summary>
    /// Formats a single node with the specified style and writes the tree representation to the provided writer.
    /// </summary>
    /// <param name="node">The root node to format.</param>
    /// <param name="style">The tree style to use for formatting.</param>
    /// <param name="writer">The text writer to write the formatted output to.</param>
    public void Format(TNode node, TreeStyle style, TextWriter writer)
    {
        FormatNode(node, writer, style, "", true);
    }

    // the key of Andrew's implementation: recursively format each node and its children
    // the indent is built up as we go down the tree, 
    // and the prefix is determined by whether the node is the last child of its parent
    private void FormatNode(TNode node, TextWriter writer, TreeStyle style, string indent, bool isLast)
    {
        var text = this.GetLabel(node);
        var prefix = isLast ? style.FinalBranch : style.Branch;
        writer.WriteLine($"{indent}{prefix}{text}");

        var children = this.GetChildren(node);
        var childIndent = indent + (isLast ? style.Space : style.Vertical);
        var lastIndex = children.Count - 1;
        foreach (var (child, i) in children.Select((child, i) => (child, i)))
        {
            FormatNode(child, writer, style, childIndent, i == lastIndex);
        }
    }
}


/// <summary>
/// Provides extension methods for formatting trees.
/// </summary>
public static class TreeFormatter
{
    extension(Console)
    {
        /// <summary>
        /// Writes a tree structure to the console using the specified formatter.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <param name="node">The root node to write.</param>
        /// <param name="formatter">The tree formatter to use for formatting.</param>
        public static void WriteTree<TNode>(TNode node, TreeFormatter<TNode> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);
            formatter.Format(node, Console.Out);
        }
    }
}