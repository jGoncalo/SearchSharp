namespace SearchSharp.Parser;

using SearchSharp;
using SearchSharp.Items;
using SearchSharp.Items.Expressions;
using Sprache;

public static class QueryParser {
    #region Numeric
    public static Parser<NumericLiteral> Int => 
        from sign in Parse.Char('-').Optional().Named("sign")
        from @int in Parse.Numeric.AtLeastOnce().Text().Token().Named("int-val")
        select NumericLiteral.Int( (sign.IsDefined ? "-" : "") + @int);
    public static Parser<NumericLiteral> Float => 
        from sign in Parse.Char('-').Optional().Named("sign")
        from lInt in Parse.Numeric.AtLeastOnce().Text().Token().Named("float-major")
        from dot in Parse.Char('.').Once().Named("float-dot")
        from rInt in Parse.Numeric.AtLeastOnce().Text().Token().Named("float-minor")
        select NumericLiteral.Float( (sign.IsDefined ? "-" : "") + $"{lInt}.{rInt}");
    public static Parser<NumericLiteral> Numeric => Float.Or(Int);
    #endregion

    #region String
    public static Parser<string> Identifier => 
        from leading in Parse.CharExcept("0123456789&|^><=. \t\n-[]:~").AtLeastOnce().Text()
        from trailing in Parse.LetterOrDigit.Many().Text()
        select leading + trailing;
    public static Parser<StringLiteral> String => (from empty in Parse.String("\"\"").Text().Token().Named("string-empty") select new StringLiteral(string.Empty))
        .Or(from leading in Parse.Char('"').Once().Named("string-start")
            from content in Parse.CharExcept('"').AtLeastOnce().Text().Token().Named("string-content")
            from trailing in Parse.Char('"').Once().Named("string-end")
            select new StringLiteral(content.Trim()));
    #endregion

    #region LogicOperator
    public static Parser<LogicOperator> LogicOp => from op in Parse.Chars('&', '|', '^').Once().Text().Token().Named("logic-operator")
        select op switch {
            "&" => LogicOperator.And,
            "|" => LogicOperator.Or,
            "^" => LogicOperator.Xor,

            _ => throw new Exception($"Unexpected LogicOperator: {op}")
        };
    #endregion

    #region Directive
    #region Operators
    public static Parser<DirectiveComparisonOperator> RuleDirectiveOp => from op in Parse.Chars(':', '=', '~').Once().Text().Token().Named("directive-operator")
        select op switch { 
            ":" => DirectiveComparisonOperator.Rule,
            "=" => DirectiveComparisonOperator.Equal,
            "~" => DirectiveComparisonOperator.Similar,
            _ => throw new Exception($"Unexpected RuleDirectiveOp: {op}")
         };
    public static Parser<NumericDirective.Operator> NumericDirectiveOperator => from op in Parse.Regex("<=|>=|<|>").Named("diretive-numeric-operator").Text().Token()
        from num in Numeric
        select new NumericDirective.Operator(op switch {
            ">=" => DirectiveNumericOperator.GreaterOrEqual,
            ">" => DirectiveNumericOperator.Greater,
            "<" => DirectiveNumericOperator.Lesser,
            "<=" => DirectiveNumericOperator.LesserOrEqual,
            _ => throw new Exception($"Unexpected NumericDirectiveOperator {op}")
        }, num);
    public static Parser<RangeDirective.Operator> RangeDirectiveOperator => (from leading in Parse.Char('[').Once().Named("diretive-range-start")
        from lower in Numeric.Named("diretive-range-lower")
        from mark in Parse.String("..").Once().Named("diretive-range-divider")
        from upper in Numeric.Named("diretive-range-upper")
        from trailing in Parse.Char(']').Once().Named("diretive-range-end")
        select new RangeDirective.Operator(lower, upper))
        .Or(from leading in Parse.Char('[').Once().Named("diretive-range-start")
            from lower in Numeric.Named("diretive-range-lower")
            from mark in Parse.String("..]").Once().Named("diretive-range-end")
            select new RangeDirective.Operator(lower, NumericLiteral.Max))
        .Or(from leading in Parse.String("[..").Once().Named("diretive-range-start")
            from upper in Numeric.Named("diretive-range-upper")
            from mark in Parse.Char(']').Once().Named("diretive-range-end")
            select new RangeDirective.Operator(NumericLiteral.Min, upper));
    #endregion
    
    public static Parser<Directive> Directive => 
        (from id in Identifier.Named("directive-identifier")
        from op in RuleDirectiveOp.Named("directive-operator")
        from lit in (from str in String select str as Literal).Or(from num in Numeric select num as Literal).Named("directive-value")
        select new ComparisonDirective(op, id, lit) as Directive)
        .Or(from id in Identifier.Named("directive-identifier")
            from op in RangeDirectiveOperator.Named("directive-num-operator")
            select new RangeDirective(op, id) as Directive)
        .Or(from id in Identifier.Named("directive-identifier")
            from op in NumericDirectiveOperator.Named("directive-range-operator")
            select new NumericDirective(op, id) as Directive);
    #endregion

    #region Expressions
    public static Parser<LogicExpression> LogicExpression => (
        from lP in Parse.Char('(').Named("logic-parenthesis-left")
        from logicExpr in LogicExpression.Named("logic-parenthesis-expr")
        from rP in Parse.Char(')').Named("logic-parenthesis-right")
        select logicExpr)
        .Or(from neg in Parse.Char('!').Once().Named("logic-negated-sign")
        from logicExpr in LogicExpression.Named("logic-negated-expr")
        select new NegatedExpression(logicExpr))
        .Or(from lExp in Parse.Ref(() => RuleExpression).Named("logic-left-exp")
        from op in LogicOp.Named("logic-operator")
        from rExp in Parse.Ref(() => RuleExpression).Named("logic-right-exp")
        select new BinaryExpression(op, lExp, rExp))
        .Or(from expr in Parse.Ref(() => RuleExpression).Named("exp") select expr);
    public static Parser<LogicExpression> RuleExpression => 
        (from lP in Parse.Char('(').Once().Named("rule-parenthesis-left")
            from lExp in RuleExpression.Named("rule-parenthesis-exp")
            from rP in Parse.Char(')').Once().Named("rule-parenthesis-right")
            select lExp)
        .Or(from neg in Parse.Char('!').Once().Named("rule-neg-sign")
            from lExp in RuleExpression.Named("rule-neg-exp")
            select new NegatedExpression(lExp))
        .Or(from dir in Directive.Named("rule-directive") select new DirectiveExpression(dir));

    public static Parser<StringExpression> StringExpression => (from empty in Parse.String("\"\"").Once() select new StringExpression(string.Empty))
        .Or(from leading in Parse.Char('"').Once()
            from content in Parse.CharExcept('"').AtLeastOnce().Text().Token()
            from trailing in Parse.Char('"').Once()
            select new StringExpression(content))
        .Or(from content  in Parse.AnyChar.AtLeastOnce().Text().Token() select new StringExpression(content));
    #endregion

    public static Parser<Query> Query => (from expr in LogicExpression select new Query(expr))
        .Or(from str in StringExpression select new Query(str));
}