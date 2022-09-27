using SearchSharp.Engine.Parser.Components.Literals;
using SearchSharp.Engine.Parser.Components.Directives;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser;

using SearchSharp;
using Sprache;

/// <summary>
/// Parse strings into a Dynamic Query Language (DQL)
/// </summary>
public static class QueryParser {
    #region Boolean
    /// <summary>
    /// Parse a string into a DQL Boolean
    /// </summary>
    public static Parser<BooleanLiteral> Bool => 
        (from @true in Parse.IgnoreCase("true").Once() select new BooleanLiteral(true))
        .Or(from @false in Parse.IgnoreCase("false").Once() select new BooleanLiteral(false));
    #endregion
    
    #region Numeric
    /// <summary>
    /// Parse a string as a DQL Integer
    /// </summary>
    public static Parser<NumericLiteral> Int => 
        from sign in Parse.Char('-').Optional().Named("sign")
        from @int in Parse.Numeric.AtLeastOnce().Text().Named("int-val")
        select NumericLiteral.Int( (sign.IsDefined ? "-" : "") + @int);
    /// <summary>
    /// Parse a string as a DQL Float
    /// </summary>
    public static Parser<NumericLiteral> Float => 
        from sign in Parse.Char('-').Optional().Named("sign")
        from lInt in Parse.Numeric.AtLeastOnce().Text().Named("float-major")
        from dot in Parse.Char('.').Once().Named("float-dot")
        from rInt in Parse.Numeric.AtLeastOnce().Text().Named("float-minor")
        select NumericLiteral.Float( (sign.IsDefined ? "-" : "") + $"{lInt}.{rInt}");
    /// <summary>
    /// Parse a string as a DQL Numeric
    /// </summary>
    public static Parser<NumericLiteral> Numeric => Float.Or(Int);
    #endregion

    #region String
    /// <summary>
    /// Parse a string into a DQL Identifier
    /// </summary>
    public static Parser<string> Identifier => 
        from leading in Parse.Letter.AtLeastOnce().Text()
        from trailing in Parse.CharExcept("&|^><=. \t\n[](){}:~@").Many().Text()
        select leading + trailing;
    /// <summary>
    /// Parse an escaped charracter
    /// </summary>
    public static Parser<string> EscapedString => (from escape in Parse.Char('\\').Once().Text()
        from escapped in Parse.AnyChar.Once().Text()
        select escape + escapped);
    /// <summary>
    /// Parse a string into a DQL String
    /// </summary>
    public static Parser<StringLiteral> String => (from empty in Parse.String("\"\"").Text().Named("string-empty") select new StringLiteral(string.Empty))
        .Or(from leading in Parse.Char('"').Once().Named("string-start")
            from content in ((from escaped in EscapedString select escaped))
                           .Or(from ctn in Parse.CharExcept('"') select $"{ctn}")
                           .AtLeastOnce().Named("string-content")
            from trailing in Parse.Char('"').Once().Named("string-end")
            select new StringLiteral(content.Aggregate((val, acumulator) => val + acumulator)));
    #endregion

    #region LogicOperator
    /// <summary>
    /// Parse a string into a DQL Logic Operator
    /// </summary>
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
    /// <summary>
    /// Parse a string into a DQL Rule Directive Operator
    /// </summary>
    public static Parser<DirectiveComparisonOperator> RuleDirectiveOp => from op in Parse.Chars(':', '=', '~').Once().Text().Named("directive-operator")
        select op switch { 
            ":" => DirectiveComparisonOperator.Rule,
            "=" => DirectiveComparisonOperator.Equal,
            "~" => DirectiveComparisonOperator.Similar,
            _ => throw new Exception($"Unexpected RuleDirectiveOp: {op}")
         };
    /// <summary>
    /// Parse a string into a DQL Numeric Directive Operator
    /// </summary>
    public static Parser<NumericDirective.Operator> NumericDirectiveOperator => from op in Parse.Regex("<=|>=|<|>").Named("diretive-numeric-operator").Text()
        from num in Numeric
        select new NumericDirective.Operator(op switch {
            ">=" => DirectiveNumericOperator.GreaterOrEqual,
            ">" => DirectiveNumericOperator.Greater,
            "<" => DirectiveNumericOperator.Lesser,
            "<=" => DirectiveNumericOperator.LesserOrEqual,
            _ => throw new Exception($"Unexpected NumericDirectiveOperator {op}")
        }, num);
    /// <summary>
    /// Parse a string into a DQL Range Directive Operator
    /// </summary>
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

    /// <summary>
    /// Parse a string into a DQL Numeric Arguments
    /// </summary>
    public static Parser<Arguments<NumericLiteral>> NumericArguments => (from num in Numeric
            from comma in Parse.Char(',').Once().Text().Token()
            from numArgs in NumericArguments
            select num + numArgs)
        .Or(from num in Numeric select new Arguments<NumericLiteral>(num));
    /// <summary>
    /// Parse a string into a DQL String Arguments
    /// </summary>
    public static Parser<Arguments<StringLiteral>> StringArguments => (from str in String
            from comma in Parse.Char(',').Once().Text().Token()
            from strArgs in StringArguments
            select str + strArgs)
        .Or(from str in String select new Arguments<StringLiteral>(str));
    /// <summary>
    /// Parse a string into a DQL Boolean Arguments
    /// </summary>
    public static Parser<Arguments<BooleanLiteral>> BooleanArguments => (from @bool in Bool
            from comma in Parse.Char(',').Once().Text().Token()
            from boolArgs in BooleanArguments
            select @bool + boolArgs)
        .Or(from @bool in Bool select new Arguments<BooleanLiteral>(@bool));

    /// <summary>
    /// Parse a string into a DQL Arguments (with list syntax)
    /// </summary>
    public static Parser<Arguments> ListDirectiveArguments => from leading in Parse.Char('[').Once().Named("directive-list-start")
        from arguments in (from numArgs in NumericArguments.Token().Named("directive-list-arguments") select numArgs as Arguments)
                        .Or(from strArgs in StringArguments.Token().Named("directive-list-arguments") select strArgs as Arguments)
                        .Or(from boolArgs in BooleanArguments.Token().Named("directive-list-arguments") select boolArgs as Arguments)
        from trailing in Parse.Char(']').Once().Named("directive-list-end")
        select arguments;
    #endregion
    
    /// <summary>
    /// Parse a string into a DQL Directive
    /// </summary>
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
            from op in ListDirectiveArguments.Named("directive-list-operator")
            select new ListDirective(op, id) as Directive)
        .Or(from id in Identifier.Named("directive-identifier")
            from op in NumericDirectiveOperator.Named("directive-range-operator")
            select new NumericDirective(op, id) as Directive);
    #endregion
    
    #region Commands
    /// <summary>
    /// Parse a string into a DQL Literal
    /// </summary>
    public static Parser<Literal> Literal => from lit in (from num in Numeric select num as Literal)
                                                        .Or(from str in String select str as Literal)
                                                        .Or(from @bool in Bool select @bool as Literal)
        select lit;
    /// <summary>
    /// Parse a string into a DQL Arguments (command syntax)
    /// </summary>
    public static Parser<Arguments> Arguments => (from lit in Literal
            from comma in Parse.Char(',').Once().Text().Token()
            from args in Arguments
            select lit + args)
        .Or(from lit in Literal select new Arguments(lit));

    /// <summary>
    /// Parse a string into a DQL Command
    /// </summary>
    public static Parser<Command>  CommandParser => (from prefix in Parse.Char('#').Once().Named("command-prefix")
            from id in Identifier.Named("command-identifier")
            from lP in Parse.Char('(').Once()
            from args in Arguments.Token().Named("command-arguments")
            from rP in Parse.Char(')').Once()
            select Command.WithArguments(id, args))
        .Or(from prefix in Parse.Char('#').Once().Named("command-prefix")
            from id in Identifier.Named("command-identifier")
            from lP in Parse.Char('(').Once()
            from ws in Parse.WhiteSpace.Optional()
            from rP in Parse.Char(')').Once()
            select Command.NoArgument(id))
        .Or(from prefix in Parse.Char('#').Once().Named("command-prefix")
            from id in Identifier.Named("command-identifier")
            select Command.NoArgument(id));

    #endregion
    
    #region Selector
    /// <summary>
    /// Parse a string into a DQL Provider
    /// </summary>
    public static Parser<Provider> ProviderInfo => from left in Parse.Char('<').Named("provider-selector-left")
        from provider in (from provider in Identifier.Named("provider-selector-providerId") 
                            from at in Parse.Char('@') 
                            from engine in Identifier.Named("provider-selector-engineAlias")  
                            select Provider.With(engine, provider))
                        .Or(from at in Parse.Char('@') 
                            from engine in Identifier.Named("provider-selector-engineAlias")  
                            select Provider.WithEngine(engine))
                        .Or(from provider in Identifier.Named("provider-selector-providerId") 
                            select Provider.WithProvider(provider))
        from right in Parse.Char('>').Named("provider-selector-right")
        select provider;
    #endregion

    #region Expressions
    /// <summary>
    /// Parse a string into a DQL Logic Expression
    /// </summary>
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
    /// <summary>
    /// Parse a string into a DQL Rule Expression
    /// </summary>
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
    /// <summary>
    /// Parse a string into a DQL String Expression
    /// </summary>
    public static Parser<StringExpression> StringExpression => (from empty in Parse.String("\"\"").Once() select new StringExpression(string.Empty))
        .Or(from leading in Parse.Char('"').Once()
            from content in Parse.CharExcept('"').AtLeastOnce().Text()
            from trailing in Parse.Char('"').Once()
            select new StringExpression(content))
        .Or(from content  in Parse.AnyChar.AtLeastOnce().Text().Token() select new StringExpression(content));
    /// <summary>
    /// Parse a string into a DQL Command Expression
    /// </summary>
    public static Parser<CommandExpression> CommandExpression => (from command in CommandParser.Token()
        from commandExpr in CommandExpression
        select commandExpr + command)
        .Or(from command in CommandParser.Token()
            select new CommandExpression(command));
    #endregion

    /// <summary>
    /// Parse a string into a DQL Constraint
    /// </summary>
    public static Parser<Constraint> Constraint => from expression in (from lExpr in LogicExpression select lExpr as Expression).Or(from sExpr in StringExpression select sExpr as Expression)
        select new Constraint(expression);

    /// <summary>
    /// Parse a string into a DQL Query (with meta instructions)
    /// </summary>
    public static Parser<Query> MetaQuery => (from provider in ProviderInfo.Token()
        from CommandExpression in CommandExpression.Token()
        select new Query(provider, CommandExpression, Components.Constraint.None))
        .Or(from provider in ProviderInfo.Token() select new Query(provider, Components.Expressions.CommandExpression.Empty, Components.Constraint.None))
        .Or(from CommandExpression in CommandExpression.Token() select new Query(Provider.None, CommandExpression, Components.Constraint.None));

    /// <summary>
    /// Parse a string into a DQL Query
    /// </summary>
    public static Parser<Query> Query => 
        (from meta in MetaQuery.Token()
         from constraint in Constraint
         select new Query(meta.Provider, meta.CommandExpression, constraint))
        .Or(from meta in MetaQuery.Token() select meta)
        .Or(from constraint in Constraint select new Query(Provider.None, Components.Expressions.CommandExpression.Empty, constraint));
}