using SearchSharp.Engine.Parser;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class CommandExpressionParser {
    
    [Fact]
    public void CanHandle_One() {
        var result = QueryParser.CommandExpression.TryParse("#cmd");

        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Value.Commands);
        var only = Assert.Single(result.Value.Commands);

        Assert.Equal("cmd", only.Identifier);
    }

    [Theory]
    [InlineData("#cmd #alt", 2)]
    [InlineData("#aaa #bbb #ccc #ddd", 4)]
    [InlineData("#aaa #bbb #ccc #ddd", 4)]
    public void CanHandle_Many(string value, int total) {
        var result = QueryParser.CommandExpression.TryParse(value);

        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Value.Commands);
        Assert.NotEmpty(result.Value.Commands);
        Assert.Equal(total, result.Value.Commands.Length);
    }
}