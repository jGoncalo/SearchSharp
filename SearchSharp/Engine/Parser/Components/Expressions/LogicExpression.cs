namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL Logic Expression
/// </summary>
public abstract record LogicExpression : Expression {
    /// <summary>
    /// Instanciate a logic expression
    /// </summary>
    /// <param name="type">Type of logic expression</param>
    public LogicExpression(ExpType type) : base(type) {
        
    }

    /// <summary>
    /// Create a Negated Expression with this expression as root
    /// </summary>
    /// <returns>DQL Negated Expression</returns>
    public NegatedExpression Negate() => new NegatedExpression(this);

    /// <summary>
    /// Negates the expression
    /// </summary>
    /// <param name="exp">DQL Expression to be negated</param>
    /// <returns>DQL Negated Expression</returns>
    public static NegatedExpression operator !(LogicExpression exp) => exp.Negate();
    /// <summary>
    /// Binary Expression with AND rule
    /// </summary>
    /// <param name="left">DQL Logic Expression #1</param>
    /// <param name="right">DQL Logic Expression #2</param>
    /// <returns>Binary Expression such that: #1 AND #2</returns>
    public static BinaryExpression operator &(LogicExpression left, LogicExpression right) => new BinaryExpression(LogicOperator.And, left, right);
    /// <summary>
    /// Binary Expression with OR rule
    /// </summary>
    /// <param name="left">DQL Logic Expression #1</param>
    /// <param name="right">DQL Logic Expression #2</param>
    /// <returns>Binary Expression such that: #1 OR #2</returns>
    public static BinaryExpression operator |(LogicExpression left, LogicExpression right) => new BinaryExpression(LogicOperator.Or, left, right);
    /// <summary>
    /// Binary Expression with XOR rule
    /// </summary>
    /// <param name="left">DQL Logic Expression #1</param>
    /// <param name="right">DQL Logic Expression #2</param>
    /// <returns>Binary Expression such that: #1 XOR #2</returns>
    public static BinaryExpression operator ^(LogicExpression left, LogicExpression right) => new BinaryExpression(LogicOperator.Xor, left, right);
}