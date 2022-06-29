namespace SearchSharp.Tests.Parser;

using SearchSharp.Engine.Parser;
using SearchSharp.Items;
using SearchSharp.Items.Expressions;
using SearchSharp;
using Sprache;
using System.Text.RegularExpressions;

public class ParserTests
{
    [Theory]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("\n", false)]
    [InlineData("\t", false)]
    [InlineData("2", false)]
    [InlineData("&", false)]
    [InlineData("|", false)]
    [InlineData("^", false)]
    [InlineData("-", false)]
    [InlineData(">", false)]
    [InlineData("<", false)]
    [InlineData("=", false)]
    [InlineData("a", true)]
    [InlineData("ab", true)]
    [InlineData("a2", true)]
    [InlineData("a22", true)]
    [InlineData("_22d", true)]
    public void IdentifierParser(string text, bool shouldWork)
    {
        var output = QueryParser.Identifier.TryParse(text);

        Assert.Equal(shouldWork, output.WasSuccessful);
        if(shouldWork) Assert.Equal(text, output.Value);
    }


    [Theory]
    [InlineData(-1.55f)]
    [InlineData(-1.42f)]
    [InlineData(0.42f)]
    [InlineData(1.00f)]
    [InlineData(1.42f)]
    [InlineData(1.55f)]
    public void Float_Parser(float value)
    {
        var text = $"{value:0.00}";
        
        var output = QueryParser.Float.TryParse(text);

        Assert.True(output.WasSuccessful);
        Assert.Equal(value, output.Value.AsFloat);
        Assert.Equal((int) value, output.Value.AsInt);
        Assert.Equal(text, output.Value.RawValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Int_Parser(int value)
    {
        var text = $"{value}";
        
        var output = QueryParser.Int.TryParse(text);

        Assert.True(output.WasSuccessful);
        Assert.Equal(value, output.Value.AsInt);
        Assert.Equal((float) value, output.Value.AsFloat);
        Assert.Equal(text, output.Value.RawValue);
    }

    [Theory]
    [InlineData("-1.55")]
    [InlineData("-1.42")]
    [InlineData("0.42")]
    [InlineData("1.00")]
    [InlineData("1.42")]
    [InlineData("1.55")]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("1")]
    public void Numeric_Parser(string value)
    {
        var floatValue = float.Parse(value);
        var intValue = (int) floatValue;
        var output = QueryParser.Numeric.TryParse(value);

        Assert.True(output.WasSuccessful);
        Assert.Equal(intValue, output.Value.AsInt);
        Assert.Equal(floatValue, output.Value.AsFloat);
        Assert.Equal(value, output.Value.RawValue);
    }

    [Theory]
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
        Assert.Equal(value.Trim(), output.Value.Value);
        Assert.Equal(value.Trim(), output.Value.RawValue);

        output = QueryParser.String.TryParse(value);
        
        Assert.False(output.WasSuccessful);
    }

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
            }, result.Value.OperatorType);
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
            } else Assert.Null(result.Value.LowerBound);

            if(!string.IsNullOrWhiteSpace(upper)){
                var upperFloat = float.Parse(upper);
                Assert.NotNull(result.Value.UpperBound);
                Assert.Equal(upperFloat, result.Value.UpperBound!.AsFloat);
            } else Assert.Null(result.Value.UpperBound);
        }
        else Assert.False(result.WasSuccessful);
    }

    [Theory]
    [InlineData("fail:", null)]
    [InlineData("fail=", null)]
    [InlineData("fail~", null)]
    [InlineData("id>=", null)]
    [InlineData("id>", null)]
    [InlineData("id<", null)]
    [InlineData("id<=", null)]
    [InlineData("id[..]", null)]
    [InlineData("id<>", null)]
    [InlineData("id", null)]
    [InlineData("id:1234", DirectiveType.Comparison)]
    [InlineData("id=1234", DirectiveType.Comparison)]
    [InlineData("id~1234", DirectiveType.Comparison)]
    [InlineData("id>=1", DirectiveType.Numeric)]
    [InlineData("id>1", DirectiveType.Numeric)]
    [InlineData("id<1", DirectiveType.Numeric)]
    [InlineData("id<=1", DirectiveType.Numeric)]
    [InlineData("id[1..10]", DirectiveType.Range)]
    [InlineData("id[..10]", DirectiveType.Range)]
    [InlineData("id[1..]", DirectiveType.Range)]
    public void Directive_Parser(string value, DirectiveType? expectedType){
        var result = QueryParser.Directive.TryParse(value);

        if(expectedType == null) Assert.False(result.WasSuccessful);
        else {
            Assert.True(result.WasSuccessful);

            Assert.Equal(expectedType!, result.Value.Type);
            Assert.Equal("id", result.Value.Identifier);

            switch(result.Value){
                case ComparisonDirective spec:
                    Assert.Equal("1234", spec.Value.RawValue);
                    break;
                case NumericDirective num:
                    Assert.Equal("1", num.OperatorSpec.Value.RawValue);
                    break;
                case RangeDirective range:
                    Assert.Equal("1", range.OperatorSpec.LowerBound?.RawValue ?? "1");
                    Assert.Equal("10", range.OperatorSpec.UpperBound?.RawValue ?? "10");
                    break;
                default:
                    Assert.True(false, "unexpected directive class");
                    break;
            }
        }
    }

    [Theory]
    [InlineData("abc:123", ExpType.Directive)]
    public void RuleExpression_Parse(string value, ExpType? subType){
        var result = QueryParser.RuleExpression.TryParse(value);

        Assert.Equal(subType != null, result.WasSuccessful);
        if(subType != null) {
            switch(subType) {
                case ExpType.Directive:
                    Assert.IsType<DirectiveExpression>(result.Value);
                    break;
            }
        }
    }

    [Theory]
    [InlineData("abc:123")]
    [InlineData("abc:123&zzz:222")]
    [InlineData("(abc:123&zzz:222)")]
    public void LogicExpression_Parse(string value){
        var result = QueryParser.LogicExpression.TryParse(value);

        Assert.True(result.WasSuccessful);
    }
}