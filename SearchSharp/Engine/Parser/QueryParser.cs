using SearchSharp.Engine.Parser.Components;
using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser;

using SearchSharp;
using Sprache;

public static class QueryParser {
    #region Boolean
    public static Parser<BooleanLiteral> Bool => 
        (from @true in Parse.IgnoreCase("true").Once() select new BooleanLiteral(true))
        .Or(from @false in Parse.IgnoreCase("false").Once() select new BooleanLiteral(false));
    #endregion
    
    #region Numeric
    public static Parser<NumericLiteral> Int => 
        from sign in Parse.Char('-').Optional().Named("sign")
        from @int in Parse.Numeric.AtLeastOnce().Text().Named("int-val")
        select NumericLiteral.Int( (sign.IsDefined ? "-" : "") + @int);
    public static Parser<NumericLiteral> Float => 
        from sign in Parse.Char('-').Optional().Named("sign")
        from lInt in Parse.Numeric.AtLeastOnce().Text().Named("float-major")
        from dot in Parse.Char('.').Once().Named("float-dot")
        from rInt in Parse.Numeric.AtLeastOnce().Text().Named("float-minor")
        select NumericLiteral.Float( (sign.IsDefined ? "-" : "") + $"{lInt}.{rInt}");
    public static Parser<NumericLiteral> Numeric => Float.Or(Int);
    #endregion

    #region String
    public static Parser<string> Identifier => 
        from leading in Parse.Letter.AtLeastOnce().Text()
        from trailing in Parse.CharExcept("&|^><=. \t\n[](){}:~").Many().Text()
        select leading + trailing;
    public static Parser<StringLiteral> String => (from empty in Parse.String("\"\"").Text().Named("string-empty") select new StringLiteral(string.Empty))
        .Or(from leading in Parse.Char('"').Once().Named("string-start")
            from content in Parse.CharExcept('"').AtLeastOnce().Text().Named("string-content")
            from trailing in Parse.Char('"').Once().Named("string-end")
            select new StringLiteral(content));
    #endregion

    #region LogicOperator
    public static Parser<LogicOperator> LogicOp => from op in Parse.Chars('&', '|', '^').Once().Text().Named("logic-operator")
        select op switch {
            "&" => LogicOperator.And,
            "|" => LogicOperator.Or,
            "^" => LogicOperator.Xor,

            _ => throw new Exception($"Unexpected LogicOperator: {op}")
        };
    #endregion

    #region Directive
    #region Operators
    public static Parser<DirectiveComparisonOperator> RuleDirectiveOp => from op in Parse.Chars(':', '=', '~').Once().Text().Named("directive-operator")
        select op switch { 
            ":" => DirectiveComparisonOperator.Rule,
            "=" => DirectiveComparisonOperator.Equal,
            "~" => DirectiveComparisonOperator.Similar,
            _ => throw new Exception($"Unexpected RuleDirectiveOp: {op}")
         };
    public static Parser<NumericDirective.Operator> NumericDirectiveOperator => from op in Parse.Regex("<=|>=|<|>").Named("diretive-numeric-operator").Text()
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
        from lit in (from str in String select str as Literal)
                        .Or(from num in Numeric select num as Literal)
                        .Or(from @bool in Bool select @bool as Literal).Named("directive-value")
        select new ComparisonDirective(op, id, lit) as Directive)
        .Or(from id in Identifier.Named("directive-identifier")
            from op in RangeDirectiveOperator.Named("directive-num-operator")
            select new RangeDirective(op, id) as Directive)
        .Or(from id in Identifier.Named("directive-identifier")
            from op in NumericDirectiveOperator.Named("directive-range-operator")
            select new NumericDirective(op, id) as Directive);
    #endregion
    
    #region Commands
    public static Parser<Command.Argument> Argument => from lit in (from num in Numeric select num as Literal)
                                                                        .Or(from str in String select str as Literal)
                                                                        .Or(from @bool in Bool select @bool as Literal)
        select new Command.Argument(lit);
    public static Parser<Command.Argument[]> Arguments => (from arg in Argument
            from comma in Parse.Char(',').Once().Text().Token()
            from args in Arguments
            select args.Append(arg).ToArray())
        .Or(from arg in Argument select new [] { arg });

    public static Parser<Command>  Command => (from prefix in Parse.Char('#').Once().Named("command-prefix")
            from id in Identifier.Named("command-identifier")
            from lP in Parse.Char('(').Once()
            from args in Arguments.Token().Named("command-arguments")
            from rP in Parse.Char(')').Once()
            select new Command(id, args))
        .Or(from prefix in Parse.Char('#').Once().Named("command-prefix")
            from id in Identifier.Named("command-identifier")
            from lP in Parse.Char('(').Once()
            from ws in Parse.WhiteSpace.Optional()
            from rP in Parse.Char(')').Once()
            select new Command(id))
        .Or(from prefix in Parse.Char('#').Once().Named("command-prefix")
            from id in Identifier.Named("command-identifier")
            select new Command(id));

    #endregion
    
    #region Selector
    public static Parser<string> ProviderSelector => from left in Parse.Char('<').Named("provider-selector-left")
        from id in Identifier.Named("provider-selector-id")
        from right in Parse.Char('>').Named("provider-selector-right")
        select id;
    #endregion

    #region Expressions
    public static Parser<LogicExpression> LogicExpression => (
        from lP in Parse.Char('(').Named("logic-parenthesis-left")
        from logicExpr in LogicExpression.Token().Named("logic-parenthesis-expr")
        from rP in Parse.Char(')').Named("logic-parenthesis-right")
        select logicExpr)
        .Or(from neg in Parse.Char('!').Once().Named("logic-negated-sign")
        from logicExpr in LogicExpression.Token().Named("logic-negated-expr")
        select new NegatedExpression(logicExpr))
        .Or(from lExp in Parse.Ref(() => RuleExpression).Token().Named("logic-left-exp")
        from op in LogicOp.Token().Named("logic-operator")
        from rExp in Parse.Ref(() => RuleExpression).Token().Named("logic-right-exp")
        select new BinaryExpression(op, lExp, rExp))
        .Or(from expr in Parse.Ref(() => RuleExpression).Named("exp") select expr);
    public static Parser<LogicExpression> RuleExpression => 
        (from lP in Parse.Char('(').Once().Named("rule-parenthesis-left")
            from lExp in RuleExpression.Token().Named("rule-parenthesis-exp")
            from rP in Parse.Char(')').Once().Named("rule-parenthesis-right")
            select lExp)
        .Or(from neg in Parse.Char('!').Once().Named("rule-neg-sign")
            from lExp in RuleExpression.Token().Named("rule-neg-exp")
            select new NegatedExpression(lExp))
        .Or(from dir in Directive.Named("rule-directive") select new DirectiveExpression(dir))
        .Or(from str in String.Named("rule-string") select new StringExpression(str.Value));

    public static Parser<StringExpression> StringExpression => (from empty in Parse.String("\"\"").Once() select new StringExpression(string.Empty))
        .Or(from leading in Parse.Char('"').Once()
            from content in Parse.CharExcept('"').AtLeastOnce().Text()
            from trailing in Parse.Char('"').Once()
            select new StringExpression(content))
        .Or(from content  in Parse.AnyChar.AtLeastOnce().Text().Token() select new StringExpression(content));

    public static Parser<CommandExpression> CommandExpression => (from command in Command.Token()
        from commandExpr in CommandExpression
        select new CommandExpression(commandExpr, command))
        .Or(from command in Command.Token()
            select new CommandExpression(command));
    #endregion

    public static Parser<Query> Query => (from provider in ProviderSelector.Token() from query in Query select new Query(query.Root, provider)) 
        .Or(from commandExpr in CommandExpression.Token() from query in Query select new Query(query.Root, query.Provider, commandExpr.Commands))
        .Or(from expr in LogicExpression select new Query(expr, null))
        .Or(from str in StringExpression select new Query(str, null));
}