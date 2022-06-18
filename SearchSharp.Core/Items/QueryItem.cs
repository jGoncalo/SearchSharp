namespace SearchSharp.Core.Items;

public abstract class Item {
    
}

public abstract class Query {

}

public class QueryLiteral : Query {
    public readonly string Literal;

    public QueryLiteral(string literal) {
        Literal = literal;
    }
}

public class QueryDirective {

    public readonly string Identifier;
    public readonly string Value;

    public QueryDirective(string identifier, string value){
        Identifier = identifier;
        Value = value;
    }
}

public abstract class QueryExpression : Query {
    public readonly ExpressionType Type;
    public readonly QueryExpression? Child;

    public QueryExpression(ExpressionType type, QueryExpression? child = null) {
        Type = type;
        Child = child;
    }
}

public class DirectiveExpression : QueryExpression {
    public readonly QueryDirective Directive;

    public DirectiveExpression(QueryDirective directive) : base(ExpressionType.Directive) {
        Directive = directive;
    }
}
public class NegateExpression : QueryExpression {
    public NegateExpression(QueryExpression child) : base(ExpressionType.Negate, child) {}
}
public class BinaryExpression : QueryExpression {
    public readonly BinaryOperationType OpType;
    public readonly QueryExpression Left;
    public readonly QueryExpression Right;

    public BinaryExpression(BinaryOperationType type, QueryExpression left, QueryExpression right) : base(ExpressionType.BinaryOperation, null) {
        OpType = type;
        Left = left;
        Right = right;
    }
}