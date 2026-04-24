using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xtofs.TextUtilities;

using Xunit;

public class TableFormatterTests
{
    private record Person(string Name, int Age);

    [Fact]
    public async Task WriteTableAsync_WritesCorrectTable()
    {
        var people = new List<Person>
        {
            new("Alice", 30),
            new("Bob", 25)
        };
        var format = new TableFormat<Person>(
            new[] { "Name", "Age" },
            new List<Func<Person, string>>
            {
                p => p.Name,
                p => p.Age.ToString()
            }
        );
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        await writer.WriteTableAsync(people, format);
        var expected =
            "Name   Age  \n" +
            "-----  ---  \n" +
            "Alice  30   \n" +
            "Bob    25   \n";
        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public async Task WriteTableAsync_ThrowsOnHeaderValueSelectorMismatch()
    {
        var people = new List<Person> { new("Alice", 30) };
        var format = new TableFormat<Person>(
            new[] { "Name" },
            new List<Func<Person, string>> { p => p.Name, p => p.Age.ToString() }
        );
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        await Assert.ThrowsAsync<ArgumentException>(() => writer.WriteTableAsync(people, format));
    }
}
