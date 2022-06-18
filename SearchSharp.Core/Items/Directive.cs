namespace SearchSharp.Core.Items;

public abstract class Directive : Item {
    public readonly DirectiveType Type;
    public readonly string Identifier;

    public Directive(DirectiveType type, string identifer){
        Type = type;
        Identifier = identifer;
    }
}

public class SpecDirective : Directive {
    public readonly SpecDirectiveOperator OperatorType;
    public readonly Literal Value;
    

    public SpecDirective(SpecDirectiveOperator op, string identifer, Literal value) : base(DirectiveType.Specification, identifer) {
        OperatorType = op;
        Value = value;
    }
}

public class NumericDirective : Directive {
    public class Operator {
        public readonly DirectiveNumericOperator OperatorType;
        public readonly NumericLiteral Value;

        public Operator(DirectiveNumericOperator op, NumericLiteral value) {
            OperatorType = op;
            Value = value;
        }
    }

    public readonly Operator OperatorSpec;

    public NumericDirective(Operator operatorSpec, string identifer) : base(DirectiveType.Numeric, identifer) {
        OperatorSpec = operatorSpec;
    }
}

public class RangeDirective : Directive {
    public class Operator {
        public readonly NumericLiteral? LowerBound;
        public readonly NumericLiteral? UpperBound;

        public Operator(NumericLiteral? lower, NumericLiteral? upper) {
            LowerBound = lower;
            UpperBound = upper;
        }
    }
    public readonly Operator OperatorSpec;
    public RangeDirective(Operator operatorSpec, string identifer) : base(DirectiveType.Range, identifer) {
        OperatorSpec = operatorSpec;
    }
}