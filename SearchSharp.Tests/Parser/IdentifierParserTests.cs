using SearchSharp.Engine.Parser;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class IdentifierParserTests {
    [Theory]
    [InlineData("", null)]
    [InlineData(" ", null)]
    [InlineData("\n", null)]
    [InlineData("\t", null)]
    [InlineData("2", null)]
    [InlineData("&", null)]
    [InlineData("|", null)]
    [InlineData("^", null)]
    [InlineData("-", null)]
    [InlineData(">", null)]
    [InlineData("<", null)]
    [InlineData("=", null)]
    [InlineData("a", "a")]
    [InlineData("ab", "ab")]
    [InlineData("a2", "a2")]
    [InlineData("a22", "a22")]
    [InlineData("a_23", "a_23")]
    [InlineData("a-23", "a-23")]
    public void IdentifierParser(string text, string expected)
    {
        var output = QueryParser.Identifier.TryParse(text);

        if(expected == null) Assert.False(output.WasSuccessful);
        else {
            Assert.True(output.WasSuccessful);
            Assert.Equal(expected, output.Value);
        }
    }
}