namespace SearchSharp;

/// <summary>
/// Execution time of a command
/// </summary>
[Flags]
public enum EffectiveIn {
    /// <summary>
    /// Never executes
    /// </summary>
    None = 0,
    /// <summary>
    /// Execute before constraint is applyed
    /// </summary>
    Provider = 1,
    /// <summary>
    /// Execute after constraint has been applyed
    /// </summary>
    Query = 2
}

/// <summary>
/// Type of a literal
/// </summary>
public enum LiteralType {
    /// <summary>
    /// Represents a number, can be positive or negative, float or integer
    /// </summary>
    Numeric = 0,
    /// <summary>
    /// Represents a string
    /// </summary>
    String = 1,
    /// <summary>
    /// Represents a boolean value
    /// </summary>
    Boolean = 2
}

/// <summary>
/// Expression type
/// </summary>
public enum ExpType {
    /// <summary>
    /// No expression
    /// </summary>
    None = 0,
    /// <summary>
    /// Expression that will match a string constraint
    /// </summary>
    String = 1,
    /// <summary>
    /// Expression that will match a directive constraint
    /// </summary>
    Directive = 2,
    /// <summary>
    /// Expression that is negated
    /// </summary>
    Negated = 3,
    /// <summary>
    /// Expression that represents a logic test
    /// </summary>
    Logic = 4,
    /// <summary>
    /// Expression containing multiple command invocations
    /// </summary>
    Command = 5
}

/// <summary>
/// Directive Type
/// </summary>
public enum DirectiveType {
    /// <summary>
    /// A comparison directive (=,~,:)
    /// </summary>
    Comparison = 0,
    /// <summary>
    /// A numeric directive (&#62;=,&#62;,&#60;,&#60;=)
    /// </summary>
    Numeric = 1,
    /// <summary>
    /// A range directive ([1..5],[..2],[1..])
    /// </summary>
    Range = 2,
    /// <summary>
    /// A list directive ([1,2,3,4], ["abc","def"], [true, false])
    /// </summary>
    List = 3
}

/// <summary>
/// Comparison directive operator type
/// </summary>
public enum DirectiveComparisonOperator {
    /// <summary>
    /// ':'
    /// </summary>
    Rule = 0,
    /// <summary>
    /// =
    /// </summary>
    Equal = 1,
    /// <summary>
    /// ~
    /// </summary>
    Similar = 2,
}

/// <summary>
/// Numeric directive operator
/// </summary>
public enum DirectiveNumericOperator {
    /// <summary>
    /// '&#62;='
    /// </summary>
    GreaterOrEqual = 0,
    /// <summary>
    /// '&#62;'
    /// </summary>
    Greater = 1,
    /// <summary>
    /// '&#60;'
    /// </summary>
    Lesser = 2,
    /// <summary>
    /// '&#60;='
    /// </summary>
    LesserOrEqual = 3,
}

/// <summary>
/// Logic Expression operator type
/// </summary>
public enum LogicOperator {
    /// <summary>
    /// '|' or operator
    /// </summary>
    Or = 0,
    /// <summary>
    /// '&#38;' and operator
    /// </summary>
    And = 1,
    /// <summary>
    /// '^' exclusive or operator
    /// </summary>
    Xor = 2
}