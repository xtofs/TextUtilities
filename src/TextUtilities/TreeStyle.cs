namespace TextUtilities;

/// <summary>
/// Defines the visual style for rendering tree structures using ASCII or Unicode characters.
/// </summary>
/// <param name="Indent">Characters used for indentation levels.</param>
/// <param name="Branch">Characters used for non-final branch connectors.</param>
/// <param name="FinalBranch">Characters used for the final branch connector.</param>
/// <param name="Vertical">Characters used for vertical line continuation.</param>
/// <param name="Space">Characters used for empty spacing.</param>
public record struct TreeStyle(
    string Indent,      // "  "
    string Branch,      // "├─"
    string FinalBranch, // "└─"
    string Vertical,    // "│ "
    string Space)       // "  "
{

#if DEBUG
    readonly bool _valid = Validate(Indent, Branch, FinalBranch, Vertical, Space);

    private static bool Validate(params string[] strings)
    {
        return strings.Select(s => s.Length).AllEqual() ? true :
            throw new ArgumentException("All TreeStyle components must have the same length. " +
                $"Received lengths: {string.Join(", ", strings.Select(s => $"'{s}' {s.Length}"))}");
    }

#endif

    /// <summary>
    /// Gets the default tree style (ASCII).
    /// </summary>
    public static TreeStyle Default => Ascii;

    /// <summary>
    /// Gets the ASCII tree style.
    /// </summary>
    public static TreeStyle Ascii { get; } = new TreeStyle(
        Indent: "   ",
        Branch: "|--",
        FinalBranch: "+--",
        Vertical: "|  ",
        Space: "   "
    );

    /// <summary>
    /// Gets the Unicode tree style.
    /// </summary>
    public static TreeStyle Unicode { get; } = new TreeStyle(
        Indent: "  ",
        Branch: "├─",
        FinalBranch: "└─",
        Vertical: "│ ",
        Space: "  "
    );

    /// <summary>
    /// Gets the wide ASCII tree style.
    /// </summary>
    public static TreeStyle AsciiWide { get; } = new TreeStyle(
        Indent: "    ",
        Branch: "|-- ",
        FinalBranch: "+-- ",
        Vertical: "|   ",
        Space: "    "
     );

}
