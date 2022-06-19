namespace SearchSharp;

public enum LiteralType {
    Numeric = 0,
    String = 1
}

public enum ExpressionType {
    String = 0,
    Directive = 1,
    Negated = 2,
    Logic = 3
}

public enum DirectiveType {
    Specification = 0,
    Numeric = 1,
    Range = 2
}
public enum RuleDirectiveOperator {
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

public enum LogicOperator {
    Or = 0,
    And = 1,
    Xor = 2
}