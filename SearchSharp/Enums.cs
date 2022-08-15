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
    None = 0,
    String = 1,
    Directive = 2,
    Negated = 3,
    Logic = 4,
    Command = 5
}

public enum DirectiveType {
    Comparison = 0,
    Numeric = 1,
    Range = 2,
    List = 3
}
public enum DirectiveComparisonOperator {
    Rule = 0,               //:
    Equal = 1,              //=
    Similar = 2,            //~
}

public enum DirectiveNumericOperator {
    GreaterOrEqual = 0,     //>=
    Greater = 1,            //>
    Lesser = 2,             //<
    LesserOrEqual = 3,      //<=
}

public enum LogicOperator {
    Or = 0,                 //|
    And = 1,                //&
    Xor = 2                 //^
}