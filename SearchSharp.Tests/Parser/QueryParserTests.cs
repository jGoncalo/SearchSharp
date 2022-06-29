using SearchSharp.Engine.Parser;
using SearchSharp.Items.Expressions;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class QueryParserTests {
    [Fact]
    public void CanHandle_NoCommand_String(){
        var result = QueryParser.Query.TryParse("this is a query");

        Assert.True(result.WasSuccessful);
        Assert.IsType<StringExpression>(result.Value.Root);
        Assert.NotNull(result.Value.Commands);
        Assert.Empty(result.Value.Commands);
    }

    [Fact]
    public void CanHandle_NoCommand_Logic(){
        var result = QueryParser.Query.TryParse("users=2 & length[2..]");

        Assert.True(result.WasSuccessful);
        Assert.IsAssignableFrom<LogicExpression>(result.Value.Root);
        Assert.NotNull(result.Value.Commands);
        Assert.Empty(result.Value.Commands);
    }

    [Fact]
    public void CanHandle_Commands_String(){
        var result = QueryParser.Query.TryParse("#preload #force this is a query");

        Assert.True(result.WasSuccessful);
        Assert.IsType<StringExpression>(result.Value.Root);
        Assert.NotNull(result.Value.Commands);
        Assert.NotEmpty(result.Value.Commands);
    }

    [Fact]
    public void CanHandle_Commands_Logic(){
        var result = QueryParser.Query.TryParse("#preload #force length[2..]");

        Assert.True(result.WasSuccessful);
        Assert.IsAssignableFrom<LogicExpression>(result.Value.Root);
        Assert.NotNull(result.Value.Commands);
        Assert.NotEmpty(result.Value.Commands);
    }
}