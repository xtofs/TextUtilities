using Xtofs.TextUtilities;
namespace TextUtilities.Tests;

public class StringExtensionsTests
{
    [Theory]
    [ClassData(typeof(TruncateDefaultCases))]
    public void Truncate_ReturnsExpectedResult_ForGivenCase(
        string? value,
        int maxLength,
        EllipsisStyle style,
        string? expected)
    {
        var result = value.Truncate(maxLength, style);

        Assert.Equal(expected, result);

        if (value is not null && value.Length > maxLength)
        {
            Assert.True((result?.Length ?? 0) <= maxLength, "Result length should not exceed maxLength when truncation occurs.");
        }
    }
}

public class TruncateDefaultCases : TheoryData<string?, int, EllipsisStyle, string?>
{
    public TruncateDefaultCases()
    {
        Add(null, 10, EllipsisStyle.None, null);

        Add("", 5, EllipsisStyle.None, "");
        Add("Hi", 5, EllipsisStyle.None, "Hi");
        Add("Hello world", 5, EllipsisStyle.None, "Hello");

        Add("Hi", 8, EllipsisStyle.Ascii, "Hi");
        Add("Hello world", 8, EllipsisStyle.Ascii, "Hello...");

        Add("Hi", 6, EllipsisStyle.Unicode, "Hi");
        Add("Hello world", 6, EllipsisStyle.Unicode, "Hello…");
    }
}