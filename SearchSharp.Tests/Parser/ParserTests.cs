using SearchSharp.Engine.Parser.Components.Directives;
using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Tests.Parser;

using SearchSharp.Engine.Parser;
using Sprache;

public class ParserTests
{
    [Theory]
    [InlineData("abc:123")]
    [InlineData("abc:123&zzz:222")]
    [InlineData("(abc:123&zzz:222)")]
    public void LogicExpression_Parse(string value){
        var result = QueryParser.LogicExpression.TryParse(value);

        Assert.True(result.WasSuccessful);
    }
}