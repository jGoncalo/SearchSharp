using SearchSharp.Engine.Parser;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class StringLiteralParserTests {
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("abc123")]
    [InlineData("123abc")]
    [InlineData("1a2b3c")]
    [InlineData(" abc")]
    [InlineData(" 123")]
    [InlineData(" abc123")]
    [InlineData(" 123abc")]
    [InlineData(" 1a2b3c")]
    [InlineData(" abc ")]
    [InlineData(" 123 ")]
    [InlineData(" abc123 ")]
    [InlineData(" 123abc ")]
    [InlineData(" 1a2b3c ")]
    [InlineData("123abc ")]
    [InlineData("1a2b3c ")]
    [InlineData(" 1 a 2 b 3 c ")]
    public void String_Parser(string value){
        var output = QueryParser.String.TryParse($"\"{value}\"");

        Assert.True(output.WasSuccessful);
        Assert.Equal(value, output.Value.Value);
        Assert.Equal(value, output.Value.RawValue);

        output = QueryParser.String.TryParse(value);
        
        Assert.False(output.WasSuccessful);
    }

    [Fact]
    public void EscapedCharacter_Parser() {
        var input = "\\\"";
        var output = QueryParser.EscapedString.TryParse(input);

        Assert.True(output.WasSuccessful);
        Assert.Equal(input, output.Value);
    }

    [Fact]
    public void String_Escape_Parser(){
        var input = "This is an \\\"escaped\\\" sequence";
        var output = QueryParser.String.TryParse($"\"{input}\"");

        Assert.True(output.WasSuccessful);
        Assert.Equal(input, output.Value.Value);
        Assert.Equal(input, output.Value.RawValue);
    }
}