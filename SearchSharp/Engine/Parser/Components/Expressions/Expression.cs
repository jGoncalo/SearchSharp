namespace SearchSharp.Engine.Parser.Components.Expressions;

public abstract record Expression(ExpType Type) : QueryItem {
    public Constraint AsConstraint() => new Constraint(this);

    public static Expression None => new EmptyExpression();
}