using System.Diagnostics.CodeAnalysis;

namespace TextUtilities.Tests;

public class TreeFormatterTests
{
    [Theory]
    [ClassData(typeof(TreeFormatterCases))]
    public void Format_ReturnsExpectedTree_ForGivenStyle(TreeStyle style, string treeTerm, string expected)
    {
        // arrange

        var formatter = new TreeFormatter<TestNode>(
            node => node.Label,
            node => node.Children);

        var root = TestNode.Parse(treeTerm);
        using var writer = new StringWriter() { NewLine = "\n" };

        // act  
        formatter.Format(root, style, writer);

        var actual = writer.ToString();

        // assert
        Assert.Equal(expected, actual);
    }
}

public class TreeFormatterCases : TheoryData<TreeStyle, string, string>
{
    public TreeFormatterCases()
    {
        const string sampleTree = "root(left(left.1),right)";

        var ascii = """
        +--root
           |--left
           |  +--left.1
           +--right
        """;

        var unicode = """
        └─root
          ├─left
          │ └─left.1
          └─right
        """;

        Add(TreeStyle.Default, sampleTree, ascii + "\n"); // add the line break to compensate for roaw string removing the trailing newline
        Add(TreeStyle.Ascii, sampleTree, ascii + "\n"); // add the line break to compensate for roaw string removing the trailing newline
        Add(TreeStyle.Unicode, sampleTree, unicode + "\n"); // add the line break to compensate for roaw string removing the trailing newline
    }
}
public sealed record TestNode(string Label, IReadOnlyCollection<TestNode> Children) : IParsable<TestNode>
{
    public static TestNode Parse(string input, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        var index = 0;
        var root = ParseNode();
        SkipWhitespace();
        if (index != input.Length)
        {
            throw new FormatException($"Unexpected trailing content at index {index}.");
        }

        return root;

        TestNode ParseNode()
        {
            SkipWhitespace();
            var label = ParseLabel();
            SkipWhitespace();

            var children = new List<TestNode>();
            if (Peek('('))
            {
                index++;
                SkipWhitespace();
                if (!Peek(')'))
                {
                    while (true)
                    {
                        children.Add(ParseNode());
                        SkipWhitespace();
                        if (Peek(','))
                        {
                            index++;
                            continue;
                        }

                        break;
                    }
                }

                if (!Peek(')'))
                {
                    throw new FormatException($"Expected ')' at index {index}.");
                }

                index++;
            }

            return new TestNode(label, children);
        }

        string ParseLabel()
        {
            var start = index;
            while (index < input.Length)
            {
                var ch = input[index];
                if (char.IsLetterOrDigit(ch) || ch == '.' || ch == '_' || ch == '-')
                {
                    index++;
                    continue;
                }

                break;
            }

            if (index == start)
            {
                throw new FormatException($"Expected node label at index {index}.");
            }

            return input[start..index];
        }

        void SkipWhitespace()
        {
            while (index < input.Length && char.IsWhiteSpace(input[index]))
            {
                index++;
            }
        }

        bool Peek(char expected)
        {
            return index < input.Length && input[index] == expected;
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TestNode result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s, nameof(s));
        result = Parse(s, provider);
        return true;
    }
}
