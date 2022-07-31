using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Parser.Components;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class ArgumentsParserTests {
    [Theory]
    [InlineData("12", typeof(NumericLiteral))]
    [InlineData("-12", typeof(NumericLiteral))]
    [InlineData("12.5", typeof(NumericLiteral))]
    [InlineData("-12.5", typeof(NumericLiteral))]
    [InlineData("\"someValue\"", typeof(StringLiteral))]
    public void SingleArgument(string text, Type litType){
        var argResult = QueryParser.Literal.TryParse(text);

        Assert.True(argResult.WasSuccessful);
        Assert.IsType(litType, argResult.Value);

        var argsResult = QueryParser.Arguments.TryParse(text);

        Assert.True(argsResult.WasSuccessful);
        Assert.NotNull(argsResult.Value);
        var singleArg = Assert.Single(argsResult.Value.Literals);
        Assert.IsType(litType, singleArg);
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
        Assert.NotEmpty(result.Value.Literals);
        Assert.Equal(2, result.Value.Literals.Length);
    }
}