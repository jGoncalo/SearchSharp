using SearchSharp.Engine.Parser.Components.Expressions;
using SearchSharp.Exceptions;

namespace SearchSharp.Engine.Parser.Components;

public abstract class Directive : QueryItem {
    public readonly DirectiveType Type;
    public readonly string Identifier;

    public Directive(DirectiveType type, string identifer){
        Type = type;
        Identifier = identifer;
    }

    public DirectiveExpression AsExpression() => new DirectiveExpression(this);

    public override bool Equals(object? obj)
    {
        if(obj is not Directive dir) return false;
        if(dir.Identifier != this.Identifier) return false;
        if(dir.Type != this.Type) return false;

        return this.Type switch {
            DirectiveType.Comparison => (dir as ComparisonDirective)!.Equals(this as ComparisonDirective),
            DirectiveType.Numeric => (dir as NumericDirective)!.Equals(this as NumericDirective),
            DirectiveType.Range => (dir as RangeDirective)!.Equals(this as RangeDirective),
            DirectiveType.List => (dir as ListDirective)!.Equals(this as ListDirective),

            _ => false
        };
    }

    public override int GetHashCode() => HashCode.Combine(Identifier, Type);
}

public class ComparisonDirective : Directive {
    public readonly DirectiveComparisonOperator OperatorType;
    public readonly Literal Value;
    

    public ComparisonDirective(DirectiveComparisonOperator op, string identifer, Literal value) : base(DirectiveType.Comparison, identifer) {
        OperatorType = op;
        Value = value;
    }

    public override string ToString() {
        var opStr = OperatorType switch {
            DirectiveComparisonOperator.Rule => ":",
            DirectiveComparisonOperator.Equal => "=",
            DirectiveComparisonOperator.Similar => "~",

            _ => OperatorType.ToString()
        };

        return Identifier + opStr + Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        if(obj is not ComparisonDirective dir) return false;
        
        return dir.OperatorType == OperatorType
            && dir.Value == Value;
    }

    public override int GetHashCode() => HashCode.Combine(Identifier, Type, OperatorType, Value);
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

    public override string ToString() {
        var opStr = OperatorSpec.OperatorType switch {
            DirectiveNumericOperator.Greater => ">",
            DirectiveNumericOperator.GreaterOrEqual => ">=",
            DirectiveNumericOperator.LesserOrEqual => "<=",
            DirectiveNumericOperator.Lesser => "<",

            _ => OperatorSpec.OperatorType.ToString()
        };

        return Identifier + opStr + OperatorSpec.ToString();
    }

    public override bool Equals(object? obj)
    {
        if(obj is not NumericDirective dir) return false;
        
        return dir.OperatorSpec != OperatorSpec;
    }

    public override int GetHashCode() => HashCode.Combine(Identifier, Type, OperatorSpec);
}

public class RangeDirective : Directive {
    public class Operator {
        public readonly NumericLiteral LowerBound;
        public readonly NumericLiteral UpperBound;

        public Operator(NumericLiteral lower, NumericLiteral upper) {
            LowerBound = lower;
            UpperBound = upper;
        }
    }
    public readonly Operator OperatorSpec;
    public RangeDirective(Operator operatorSpec, string identifer) : base(DirectiveType.Range, identifer) {
        OperatorSpec = operatorSpec;
    }

    public override string ToString() {
        return Identifier + $"[{OperatorSpec.LowerBound}..{OperatorSpec.UpperBound}]";
    }

    public override bool Equals(object? obj)
    {
        if(obj is not RangeDirective dir) return false;
        
        return dir.OperatorSpec != OperatorSpec;
    }

    public override int GetHashCode() => HashCode.Combine(Identifier, Type, OperatorSpec);
}

public class ListDirective : Directive {
    public readonly Arguments Arguments;

    public bool IsStringList => Arguments.IsStringList;
    public bool IsNumericList => Arguments.IsNumericList;
    public bool IsBooleanList => Arguments.IsBooleanList;

    public ListDirective(Arguments arguments, string identifier) : base(DirectiveType.List, identifier) {
        var allStringList = arguments.Literals.All(lit => lit.Type == LiteralType.String);
        var allNumericList = arguments.Literals.All(lit => lit.Type == LiteralType.Numeric);
        var allBooleanList = arguments.Literals.All(lit => lit.Type == LiteralType.Boolean);

        if(!(allStringList || allNumericList || allBooleanList)) {
            throw new ArgumentResolutionException("ListDirective arguments must be only of one type: [String, Numeric, Boolean]");
        }

        Arguments = arguments;
    }

    public override string ToString()
    {
        return Identifier + $"[{Arguments.ToString()}]";
    }

    public override bool Equals(object? obj)
    {
        if(obj is not ListDirective dir) return false;
        
        return dir.Arguments == Arguments;
    }
    public override int GetHashCode() => HashCode.Combine(Identifier, Type, Arguments);
}