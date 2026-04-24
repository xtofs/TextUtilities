namespace Xtofs.TextUtilities;

/// <summary>
/// Represents the format of a table, including headers and value selectors for each column.
/// </summary>
/// <typeparam name="T">The type of the data row.</typeparam>
public record class TableFormat<T>(string[] Headers, IReadOnlyList<Func<T, string>> ValueSelectors)
{


    /// <summary>
    /// The headers for the table columns.
    /// </summary>
    public string[] Headers { get; } = Headers;

    /// <summary>
    /// The value selector functions for each column.
    /// </summary>
    public IReadOnlyList<Func<T, string>> ValueSelectors { get; } = ValueSelectors;
}


