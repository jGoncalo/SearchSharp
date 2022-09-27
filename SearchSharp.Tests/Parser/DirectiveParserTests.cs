using System.Collections;
using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Parser.Components.Directives;
using SearchSharp.Engine.Parser.Components.Literals;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class DirectiveParserTests {
    public static class Data {
        public class Comparison : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "id", DirectiveComparisonOperator.Equal, 10.AsLiteral() };
                yield return new object[] { "id", DirectiveComparisonOperator.Equal, "abc".AsLiteral() };
                yield return new object[] { "id", DirectiveComparisonOperator.Equal, true.AsLiteral() };

                yield return new object[] { "id", DirectiveComparisonOperator.Rule, 10.AsLiteral() };
                yield return new object[] { "id", DirectiveComparisonOperator.Rule, "abc".AsLiteral() };
                yield return new object[] { "id", DirectiveComparisonOperator.Rule, true.AsLiteral() };

                yield return new object[] { "id", DirectiveComparisonOperator.Similar, 10.AsLiteral() };
                yield return new object[] { "id", DirectiveComparisonOperator.Similar, "abc".AsLiteral() };
                yield return new object[] { "id", DirectiveComparisonOperator.Similar, true.AsLiteral() };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class Numeric : IEnumerable<object[]> { 
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "id", DirectiveNumericOperator.GreaterOrEqual, 10.AsLiteral() };
                yield return new object[] { "id", DirectiveNumericOperator.GreaterOrEqual, 5.5f.AsLiteral() };

                yield return new object[] { "id", DirectiveNumericOperator.Greater, 10.AsLiteral() };
                yield return new object[] { "id", DirectiveNumericOperator.Greater, 5.5f.AsLiteral() };
                
                yield return new object[] { "id", DirectiveNumericOperator.Lesser, 10.AsLiteral() };
                yield return new object[] { "id", DirectiveNumericOperator.Lesser, 5.5f.AsLiteral() };
                
                yield return new object[] { "id", DirectiveNumericOperator.LesserOrEqual, 10.AsLiteral() };
                yield return new object[] { "id", DirectiveNumericOperator.LesserOrEqual, 5.5f.AsLiteral() };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class Range : IEnumerable<object?[]> { 
            public IEnumerator<object?[]> GetEnumerator()
            {
                yield return new object?[] { "id", null, 15.AsLiteral() };
                yield return new object?[] { "id", null, 15.5f.AsLiteral() };
                yield return new object?[] { "id", null, 15.AsLiteral() };
                yield return new object?[] { "id", null, 15.5f.AsLiteral() };

                yield return new object?[] { "id", 10.AsLiteral(), 15.AsLiteral() };
                yield return new object?[] { "id", 10.5f.AsLiteral(), 15.5f.AsLiteral() };
                yield return new object?[] { "id", 10.5f.AsLiteral(), 15.AsLiteral() };
                yield return new object?[] { "id", 10.AsLiteral(), 15.5f.AsLiteral() };

                yield return new object?[] { "id", 10.AsLiteral(), null };
                yield return new object?[] { "id", 10.5f.AsLiteral(), null };
                yield return new object?[] { "id", 10.5f.AsLiteral(), null };
                yield return new object?[] { "id", 10.AsLiteral(), null };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class List : IEnumerable<object[]> { 
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "id", new Literal[] { 10.AsLiteral(), 11.AsLiteral(), 12.AsLiteral() } };
                yield return new object[] { "id", new Literal[] { "a".AsLiteral(), "ab".AsLiteral(), "abc".AsLiteral() } };
                yield return new object[] { "id", new Literal[] { true.AsLiteral(), false.AsLiteral(), true.AsLiteral() } };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    [Theory]
    [ClassData(typeof(Data.Comparison))]
    public void Comparison_Directive_Parse(string identifier, DirectiveComparisonOperator @operator, Literal literal){
        var output = QueryParser.Directive.TryParse($"{identifier}{@operator.AsOp()}{literal}");

        Assert.True(output.WasSuccessful);
        var directive = Assert.IsType<ComparisonDirective>(output.Value);

        Assert.Equal(identifier, directive.Identifier);
        Assert.Equal(@operator, directive.Operator);
        Assert.Equal(literal, directive.Value);
    }

    [Theory]
    [ClassData(typeof(Data.Numeric))]
    public void Numeric_Directive_Parse(string identifier, DirectiveNumericOperator @operator, NumericLiteral numericLiteral) { 
        var output = QueryParser.Directive.TryParse($"{identifier}{@operator.AsOp()}{numericLiteral}");

        Assert.True(output.WasSuccessful);
        var directive = Assert.IsType<NumericDirective>(output.Value);

        Assert.Equal(identifier, directive.Identifier);
        Assert.Equal(@operator, directive.OperatorSpec.Type);
        Assert.Equal(numericLiteral, directive.OperatorSpec.Value);
    }

    [Theory]
    [ClassData(typeof(Data.Range))]
    public void Range_Directive_Parse(string identifier, NumericLiteral? lower, NumericLiteral? upper){
        var lowerStr = lower?.ToString() ?? string.Empty;
        var upperStr = upper?.ToString() ?? string.Empty;
        var output = QueryParser.Directive.TryParse($"{identifier}[{lowerStr}..{upperStr}]");

        Assert.True(output.WasSuccessful);
        var directive = Assert.IsType<RangeDirective>(output.Value);

        Assert.Equal(identifier, directive.Identifier);
        Assert.Equal(lower ?? NumericLiteral.Min, directive.OperatorSpec.LowerBound);
        Assert.Equal(upper ?? NumericLiteral.Max, directive.OperatorSpec.UpperBound);
    }

    [Theory]
    [ClassData(typeof(Data.List))]
    public void List_Directive_Parse(string identifier, Literal[] literals){
        var input = $"{identifier}[{string.Join(',', literals.Select(lit => lit.ToString()))}]";
        var output = QueryParser.Directive.TryParse(input);

        Assert.True(output.WasSuccessful);
        var directive = Assert.IsType<ListDirective>(output.Value);
        
        Assert.Equal(identifier, directive.Identifier);
        Assert.NotEmpty(directive.Arguments.Literals);
        Assert.Equal(literals.Length, directive.Arguments.Literals.Length);
        for (var i = 0; i < literals.Length; i++){
            Assert.Equal(literals[i], directive.Arguments.Literals[i]);
        }
    }
}