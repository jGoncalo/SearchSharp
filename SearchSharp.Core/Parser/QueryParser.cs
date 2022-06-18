namespace SearchSharp.Core.Parser;

using SearchSharp.Core.Items;
using SearchSharp.Core.Items.Expressions;
using Sprache;

public static class QueryParser {
    #region Numeric
    public static Parser<NumericLiteral> Int => 
        from sign in Parse.Char('-').Optional()
        from @int in Parse.Numeric.AtLeastOnce().Text().Token()
        select new NumericLiteral( (sign.IsDefined ? "-" : "") + @int, false);
    public static Parser<NumericLiteral> Float => 
        from sign in Parse.Char('-').Optional()
        from lInt in Parse.Numeric.AtLeastOnce().Text().Token()
        from dot in Parse.Char('.').Once()
        from rInt in Parse.Numeric.AtLeastOnce().Text().Token()
        select new NumericLiteral( (sign.IsDefined ? "-" : "") + $"{lInt}.{rInt}", true);
    public static Parser<NumericLiteral> Numeric => Float.Or(Int);
    #endregion

    #region String
    public static Parser<string> Identifier => Parse.Regex("[a-z]+[a-z0-9]*").Text().Token();
    public static Parser<StringLiteral> String => (from empty in Parse.String("\"\"").Text().Token() select new StringLiteral(string.Empty))
        .Or(from leading in Parse.Char('"').Once()
            from content in Parse.CharExcept('"').AtLeastOnce().Text().Token()
            from trailing in Parse.Char('"').Once()
            select new StringLiteral(content.Trim()));
    #endregion

    #region Directive
    #region Operators
    public static Parser<SpecDirectiveOperator> SpecDirectiveOperator => from op in Parse.Chars(':', '=', '~').Once().Text().Token()
        select op switch { 
            ":" => Core.SpecDirectiveOperator.Rule,
            "=" => Core.SpecDirectiveOperator.Equal,
            "~" => Core.SpecDirectiveOperator.Similar,
            _ => throw new Exception($"Unexpected SpecDirectiveOperator: {op}")
         };
    public static Parser<NumericDirective.Operator> NumericDirectiveOperator => from op in Parse.Regex("<=|>=|<|>").Text().Token()
        from num in Numeric
        select new NumericDirective.Operator(op switch {
            ">=" => DirectiveNumericOperator.GreaterOrEqual,
            ">" => DirectiveNumericOperator.Greater,
            "<" => DirectiveNumericOperator.Lesser,
            "<=" => DirectiveNumericOperator.LesserOrEqual,
            _ => throw new Exception($"Unexpected NumericDirectiveOperator {op}")
        }, num);
    public static Parser<RangeDirective.Operator> RangeDirectiveOperator => (from leading in Parse.Char('<').Once()
        from lower in Numeric
        from mark in Parse.String("..").Once()
        from upper in Numeric
        from trailing in Parse.Char('>').Once()
        select new RangeDirective.Operator(lower, upper))
        .Or(from leading in Parse.Char('<').Once()
            from lower in Numeric
            from mark in Parse.String("..>").Once()
            select new RangeDirective.Operator(lower, null))
        .Or(from leading in Parse.String("<..").Once()
            from upper in Numeric
            from mark in Parse.Char('>').Once()
            select new RangeDirective.Operator(null, upper));
    #endregion
    
    public static Parser<Directive> Directive => 
        (from id in Identifier
        from op in SpecDirectiveOperator
        from lit in (from str in String select str as Literal).Or(from num in Numeric select num as Literal)
        select new SpecDirective(op, id, lit) as Directive)
        .Or(from id in Identifier
            from op in RangeDirectiveOperator
            select new RangeDirective(op, id) as Directive)
        .Or(from id in Identifier
            from op in NumericDirectiveOperator
            select new NumericDirective(op, id) as Directive);
    #endregion

    #region Expressions
    #region Computed
    public static Parser<DirectiveExpression> DirectiveExpression => throw new NotImplementedException();
    public static Parser<NegatedExpression> NegatedExpression => throw new NotImplementedException();
    public static Parser<LogicExpression> LogicExpression => throw new NotImplementedException();

    public static Parser<ComputeExpression> ComputeExpression => throw new NotImplementedException();
    #endregion

    public static Parser<StringExpression> StringExpression => throw new NotImplementedException();
    #endregion

    public static Parser<Query> Query => (from str in StringExpression select new Query(str))
        .Or(from compute in ComputeExpression select new Query(compute));
}