using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Parser.Components.Expressions;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class QueryParserTests {
    [Fact]
    public void CanHandle_NoCommand_String(){
        var result = QueryParser.Query.TryParse("this is a query");

        Assert.True(result.WasSuccessful);
        Assert.IsType<StringExpression>(result.Value.Constraint.Root);
        Assert.NotNull(result.Value.CommandExpression.Commands);
        Assert.Empty(result.Value.CommandExpression.Commands);
    }

    [Fact]
    public void CanHandle_NoCommand_Logic(){
        var result = QueryParser.Query.TryParse("users=2 & length[2..]");

        Assert.True(result.WasSuccessful);
        Assert.IsAssignableFrom<LogicExpression>(result.Value.Constraint.Root);
        Assert.NotNull(result.Value.CommandExpression.Commands);
        Assert.Empty(result.Value.CommandExpression.Commands);
    }

    [Theory]
    [InlineData("#preload #force this is a query")]
    [InlineData("#preload #force(2) this is a query")]
    [InlineData("#preload(1) #force this is a query")]
    [InlineData("#preload(1) #force(2) this is a query")]
    public void CanHandle_Commands_String(string text){
        var result = QueryParser.Query.TryParse(text);

        Assert.True(result.WasSuccessful);
        Assert.IsType<StringExpression>(result.Value.Constraint.Root);
        Assert.NotNull(result.Value.CommandExpression.Commands);
        Assert.NotEmpty(result.Value.CommandExpression.Commands);
    }

    [Theory]
    [InlineData("a great description", ExpType.String)]
    [InlineData("!(id=2 | email~\"industry.com\")", ExpType.Negated)]
    [InlineData("length[2..]", ExpType.Directive)]
    public void CanHandle_Commands_Logic(string postfix, ExpType expectedRootType){
        var result = QueryParser.Query.TryParse("#preload #force " + postfix);

        Assert.True(result.WasSuccessful);
        Assert.Equal(expectedRootType, result.Value.Constraint.Root.Type);
        
        Assert.NotNull(result.Value.CommandExpression.Commands);
        Assert.NotEmpty(result.Value.CommandExpression.Commands);
    }
}