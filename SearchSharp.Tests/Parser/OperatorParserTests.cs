using System.Text.RegularExpressions;
using SearchSharp.Engine.Parser;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class OperatorParserTests {
    [Theory]
    [InlineData(':')]
    [InlineData('=')]
    [InlineData('~')]
    public void SpecDirectiveOperator_Parser(char op){
        var output = QueryParser.RuleDirectiveOp.TryParse($"{op}");

        Assert.True(output.WasSuccessful);
        switch(op) {
            case ':': Assert.Equal(DirectiveComparisonOperator.Rule,    output.Value); break;
            case '=': Assert.Equal(DirectiveComparisonOperator.Equal,   output.Value); break;
            case '~': Assert.Equal(DirectiveComparisonOperator.Similar, output.Value); break;
            default: Assert.True(false); break;
        }
    }

    [Theory]
    [InlineData('|', true)]
    [InlineData('&', true)]
    [InlineData('^', true)]
    [InlineData('!', false)]
    public void LogicOp_Parser(char op, bool shouldWork){
        var result = QueryParser.LogicOp.TryParse($"{op}");

        Assert.Equal(result.WasSuccessful, shouldWork);
        if(shouldWork){
            switch(op) {
                case '|': Assert.Equal(LogicOperator.Or, result.Value);  break;
                case '&': Assert.Equal(LogicOperator.And, result.Value); break;
                case '^': Assert.Equal(LogicOperator.Xor, result.Value); break;
                default:  Assert.True(false); break;
            }
        }
    }


    [Theory]
    [InlineData(">=", "1")]
    [InlineData(">", "1")]
    [InlineData("<", "1")]
    [InlineData("<=", "1")]
    [InlineData(">=", "1.3")]
    [InlineData(">", "1.3")]
    [InlineData("<", "1.3")]
    [InlineData("<=", "1.3")]
    [InlineData(">=", "")]
    [InlineData(">", "")]
    [InlineData("<", "")]
    [InlineData("<=", "")]
    [InlineData(">=", "a")]
    [InlineData(">", "b")]
    [InlineData("<", "c")]
    [InlineData("<=", "d")]
    public void NumericDirectiveOperator_Parser(string @operator, string value){
        var result = QueryParser.NumericDirectiveOperator.TryParse(@operator + value);

        if(new Regex("[0-9]+(\\.[0-9]+)?").IsMatch(value)) {
            Assert.True(result.WasSuccessful);

            Assert.Equal(@operator switch {
                ">=" => DirectiveNumericOperator.GreaterOrEqual,
                ">" => DirectiveNumericOperator.Greater,
                "<" => DirectiveNumericOperator.Lesser,
                "<=" => DirectiveNumericOperator.LesserOrEqual,
                _ => throw new Exception($"Unexpected NumericDirectiveOperator {@operator}")
            }, result.Value.Type);
        }
        else Assert.False(result.WasSuccessful);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("", "10")]
    [InlineData("1", "")]
    [InlineData("1", "10")]
    [InlineData("", "10.2")]
    [InlineData("1.5", "")]
    [InlineData("1.5", "10.2")]
    [InlineData("a", "10")]
    [InlineData("1", "a")]
    [InlineData("a", "")]
    [InlineData("", "a")]
    public void RangeDirectiveOperator_Parser(string lower, string upper){
        var result = QueryParser.RangeDirectiveOperator.TryParse($"[{lower}..{upper}]");
        var numRegEx = new Regex("[0-9]+(\\.[0-9]+)?");
        
        if(string.IsNullOrWhiteSpace(lower) && string.IsNullOrWhiteSpace(upper)) Assert.False(result.WasSuccessful);
        else if((string.IsNullOrWhiteSpace(lower) || numRegEx.IsMatch(lower)) && (string.IsNullOrWhiteSpace(upper) || numRegEx.IsMatch(upper))){
            Assert.True(result.WasSuccessful);

            if(!string.IsNullOrWhiteSpace(lower)){
                var lowerFloat = float.Parse(lower);
                Assert.NotNull(result.Value.LowerBound);
                Assert.Equal(lowerFloat, result.Value.LowerBound!.AsFloat);
            } else Assert.Equal(string.Empty, result.Value.LowerBound.RawValue);

            if(!string.IsNullOrWhiteSpace(upper)){
                var upperFloat = float.Parse(upper);
                Assert.NotNull(result.Value.UpperBound);
                Assert.Equal(upperFloat, result.Value.UpperBound!.AsFloat);
            } else Assert.Equal(string.Empty, result.Value.UpperBound.RawValue);
        }
        else Assert.False(result.WasSuccessful);
    }
}