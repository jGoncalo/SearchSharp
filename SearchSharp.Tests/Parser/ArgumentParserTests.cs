using SearchSharp.Engine.Parser;
using SearchSharp.Items;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class ArgumentParserTests {
    [Theory]
    [InlineData("12", typeof(NumericLiteral))]
    [InlineData("-12", typeof(NumericLiteral))]
    [InlineData("12.5", typeof(NumericLiteral))]
    [InlineData("-12.5", typeof(NumericLiteral))]
    [InlineData("\"someValue\"", typeof(StringLiteral))]
    public void SingleArgument(string text, Type litType){
        var argResult = QueryParser.Argument.TryParse(text);

        Assert.True(argResult.WasSuccessful);
        Assert.IsType(litType, argResult.Value.Literal);

        var argsResult = QueryParser.Arguments.TryParse(text);

        Assert.True(argsResult.WasSuccessful);
        Assert.NotNull(argsResult.Value);
        var singleArg = Assert.Single(argsResult.Value);
        Assert.IsType(litType, singleArg.Literal);
    }

    [Theory]
    [InlineData("12,12")]
    [InlineData("12 ,12")]
    [InlineData("12, 12")]
    [InlineData("12 , 12")]
    [InlineData("\"someValue\",\"someValue\"")]
    [InlineData("\"someValue\" ,\"someValue\"")]
    [InlineData("\"someValue\", \"someValue\"")]
    [InlineData("\"someValue\" , \"someValue\"")]
    [InlineData("\"someValue\",12")]
    [InlineData("\"someValue\", 12")]
    [InlineData("\"someValue\" ,12")]
    [InlineData("\"someValue\" , 12")]
    [InlineData("12,\"someValue\"")]
    [InlineData("12, \"someValue\"")]
    [InlineData("12 ,\"someValue\"")]
    [InlineData("12 , \"someValue\"")]
    public void MultipleArguments(string text){
        var result = QueryParser.Arguments.TryParse(text);

        Assert.True(result.WasSuccessful);
        Assert.NotEmpty(result.Value);
        Assert.Equal(2, result.Value.Length);
    }
}