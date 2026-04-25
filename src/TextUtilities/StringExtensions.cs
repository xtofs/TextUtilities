namespace Xtofs.TextUtilities;



/// <summary>
/// Provides helper methods for common text transformations.
/// </summary>
public static class StringExtensions
{

    /// <summary>
    /// Truncates the string to the specified maximum length, optionally appending an ellipsis.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum length of the resulting string, including the ellipsis if present.</param>
    /// <param name="ellipsisStyle">The style of ellipsis to append when truncating.</param>
    /// <returns>The truncated string, or the original string if it is shorter than <paramref name="maxLength"/>.</returns>
    public static string? Truncate(this string? value, int maxLength, EllipsisStyle ellipsisStyle = EllipsisStyle.None)
    {
        if (string.IsNullOrEmpty(value)) return value;
        if (value.Length <= maxLength) return value;

        if (ellipsisStyle == EllipsisStyle.None)
        {
            return value[..maxLength];
        }

        var ellipsis = ellipsisStyle == EllipsisStyle.Unicode ? "…" : "...";
        return value[..(maxLength - ellipsis.Length)] + ellipsis;
    }

    /// <summary>
    /// Replaces control characters (ASCII &lt;= 32) in the string with their Unicode Control Pictures equivalents.
    /// </summary>
    /// <param name="value">The string in which to replace control characters.</param>
    /// <returns>A new string with control characters replaced, or the original string if it is null or empty.</returns>
    public static string ReplaceControlCharacters(this string value)
    {
        if (string.IsNullOrEmpty(value)) { return value; }
        var result = new char[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            result[i] = ch <= 32 ? (char)(ch + 0x2400) : ch;
        }
        return new string(result);
    }
}

/// <summary>
/// Specifies the style of ellipsis to append when truncating text.
/// </summary>
public enum EllipsisStyle
{
    /// <summary>No ellipsis is appended.</summary>
    None,
    /// <summary>ASCII ellipsis (...) is appended.</summary>
    Ascii,
    /// <summary>Unicode ellipsis (…) is appended.</summary>
    Unicode
}
