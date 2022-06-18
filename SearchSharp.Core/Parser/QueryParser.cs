namespace SearchSharp.Core.Parser;

using SearchSharp.Core.Items;
using System.Collections.Generic;
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

    public static Parser<string> Num => Parse.Numeric.AtLeastOnce().Text().Token();
    public static Parser<string> Var => Parse.Letter.AtLeastOnce().Text().Token();
    public static Parser<QueryLiteral> Str => 
        (from content in Parse.LetterOrDigit.AtLeastOnce().Text().Token()
         select new QueryLiteral(content))
        .Or(from leading in Parse.Char('"').Once()
            from content in Parse.CharExcept('"').AtLeastOnce().Text().Token()
            from trailing in Parse.Char('"').Once()
            select new QueryLiteral(content))
        .Or(from empty in Parse.String("\"\"").Once()
            select new QueryLiteral(string.Empty));
    
    public static Parser<Query> Query => 
        (from literalQuery in Str
         select literalQuery as Query)
        .Or(from expresionQuery in Expression
            select expresionQuery as Query);

    public static Parser<BinaryExpression> BinaryExpression =>
        from lExp in BinaryExpression
        from bOp in Bop
        from rExp in Expression
        select new BinaryExpression(bOp, lExp, rExp);

    public static Parser<QueryExpression> Expression => 
        (from neg in Parse.Char('!').Once()
        from exp in Expression
        select new NegateExpression(exp) as QueryExpression)

        .Or(from leading in Parse.Char('(').Once()
        from exp in BinaryExpression
        from trailing in Parse.Char(')').Once()
        select exp)

        .Or(from dir in Dir
        select new DirectiveExpression(dir) as QueryExpression)

        .Or(from lExp in Expression
        from orOp in Parse.Char('|').Once()
        from rExp in Expression
        select new BinaryExpression(BinaryOperationType.Or, lExp, rExp) as QueryExpression)

/*
        .Or(from lExp in Expression
        from op in Bop
        from rExp in Expression
        select new BinaryExpression(op, lExp, rExp) as QueryExpression)
*/
        ;

    public static Parser<QueryDirective> Dir => 
        from leading in Parse.WhiteSpace.Many()
        from id in Var
        from sep in Parse.Char(':').Once()
        from val in Str
        from trailing in Parse.WhiteSpace.Many()
        select new QueryDirective(id, val.Literal);

    public static Parser<BinaryOperationType> Bop => Parse.Char('|').Once().Select(_ => BinaryOperationType.Or)
        .Or(Parse.Char('&').Once().Select(_ => BinaryOperationType.And));

    public static bool TryParse(string query, out string errorMessage, out Query parsedQuery) {
        var result = Query.TryParse(query);
        errorMessage = result.Message;
        parsedQuery = result.WasSuccessful ? result.Value : new QueryLiteral(string.Empty);

        return result.WasSuccessful;
    }
    public static bool TryParse(string query,out Query parsedQuery) {
        return TryParse(query, out var ignore, out parsedQuery);
    }
}