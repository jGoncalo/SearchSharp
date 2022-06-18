namespace SearchSharp.Core;

public enum LiteralType {
    Numeric = 0,
    String = 1
}

public enum ExpressionType {
    Negate = 0,
    Directive = 1,
    BinaryOperation = 2
}

public enum DirectiveType {
    Specification = 0,
    Numeric = 1,
    Range = 2
}
public enum SpecDirectiveOperator {
    Rule = 0,           //:
    Equal = 1,          //=
    Similar = 2,        //~
}
public enum DirectiveNumericOperator {
    GreaterOrEqual = 0,    //>=
    Greater = 1,        //>
    Lesser = 2,         //<
    LesserOrEqual = 3,     //<=
}

public enum BinaryOperationType {
    Or = 0,
    And = 1
}