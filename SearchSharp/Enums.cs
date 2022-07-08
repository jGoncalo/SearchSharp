namespace SearchSharp;

[Flags]
public enum EffectiveIn {
    None = 0,
    Provider = 1,
    Query = 2
}


public enum LiteralType {
    Numeric = 0,
    String = 1,
    Boolean = 2
}

public enum ExpType {
    String = 0,
    Directive = 1,
    Negated = 2,
    Logic = 3,
    Command = 4
}

public enum DirectiveType {
    Comparison = 0,
    Numeric = 1,
    Range = 2
}
public enum DirectiveComparisonOperator {
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