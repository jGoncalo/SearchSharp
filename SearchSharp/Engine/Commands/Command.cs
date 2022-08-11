using System.Reflection;
using SearchSharp.Attributes;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Exceptions;

namespace SearchSharp.Engine.Commands;

public class Command<TQueryData, TDataStructure, TCommandSpec> : Command<TQueryData, TDataStructure>
    where TQueryData : QueryData
    where TDataStructure : class
    where TCommandSpec : CommandTemplate<TQueryData, TDataStructure>, new() {
    
    private static LiteralType ToLiteralType(PropertyInfo propInfo){
        var type = propInfo.PropertyType;

        if(type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return LiteralType.Numeric;
        if(type == typeof(string)) return LiteralType.String;
        if(type == typeof(bool)) return LiteralType.Boolean;
        if(type.IsEnum) return LiteralType.String;

        var possibleVals = string.Join(",", Enum.GetValues<LiteralType>());
        throw new ArgumentResolutionException($"Could not translate {typeof(TCommandSpec).Name}.{propInfo.Name}:{type.Name} " +
            $"to LiteralType [{possibleVals}]");
    }

    private static string GetIdentifier() {
        var attribute = typeof(TCommandSpec)
            .GetCustomAttributes(typeof(CommandAttribute), false)
            .Cast<CommandAttribute>()
            .FirstOrDefault();
        return attribute?.Name ?? typeof(TCommandSpec).Name;
    }
    private static EffectiveIn GetEffectiveIn() {
        var attribute = typeof(TCommandSpec)
            .GetCustomAttributes(typeof(ArgumentAttribute), false)
            .Cast<CommandAttribute>()
            .FirstOrDefault();

        return attribute?.ExecuteAt ?? EffectiveIn.Query;
    }
    private static ArgumentDeclaration[] GetArguments() {
        var argList = new List<ArgumentDeclaration>();

        var propInfos = typeof(TCommandSpec).GetProperties()
            .Select(prop => {
                var attribute = prop.GetCustomAttributes(typeof(ArgumentAttribute), false)
                    .Cast<ArgumentAttribute>().FirstOrDefault();
                return new {
                    Property = prop,
                    Attribute = attribute
                };
            })
            .OrderBy(info => info.Attribute?.Position ?? Int16.MaxValue)
            .ThenBy(info => info.Attribute?.Name ?? info.Property.Name)
            .ToArray();

        foreach(var info in propInfos){
            if(!(info.Property.GetSetMethod()?.IsPublic ?? false))
                throw new ArgumentResolutionException($"Could not map {typeof(TCommandSpec).Name}.{info.Property.Name}:{info.Property.PropertyType.Name} " +
                                                        $"with no public setter");

            argList.Add(new ArgumentDeclaration(
                info.Attribute?.Name ?? info.Property.Name,
                ToLiteralType(info.Property)
            ));
        }

        return argList.ToArray();
    }
    
    private static void SetProperty(TCommandSpec instance, int argIndex, Argument argument){
        var targetProp = typeof(TCommandSpec).GetProperties()
            .FirstOrDefault(prop => prop.GetCustomAttributes(typeof(ArgumentAttribute), false)
                                        .Cast<ArgumentAttribute>()
                                        .Any(attr => attr.Name == argument.Identifier)
                                    || prop.Name == argument.Identifier);

        if(targetProp == null) return;

        var propType = targetProp.PropertyType;
        object propValue;
        
        if(propType == typeof(string)) {
            if(argument.Literal is not StringLiteral strLit) 
                throw new ArgumentResolutionException($"expected argument[{argIndex}] to be of type: {LiteralType.String} but found uncastable[{targetProp.Name}]: {propType}");
            propValue = strLit.Value;
        }
        else if (propType.IsEnum) {
            switch (argument.Literal) {
                case StringLiteral strLit:
                    Enum.TryParse(propType, strLit.Value, true, out var enumVal);
                    propValue = enumVal!;
                    break;
                case NumericLiteral numLit: {
                    var isDefined = Enum.IsDefined(propType, numLit.AsInt);
                    propValue = Enum.ToObject(propType, isDefined ? numLit.AsInt : 0);
                    break;
                }
                default:
                    throw new ArgumentResolutionException($"expected argument[{argIndex}] to be of type: {LiteralType.String} or {LiteralType.Numeric} but found uncastable[{targetProp.Name}]: {propType}");
            }
        }
        else if (propType == typeof(int)) {
            if(argument.Literal is not NumericLiteral numLit) throw new ArgumentResolutionException($"expected argument[{argIndex}] to be of type: {LiteralType.Numeric} but found uncastable[{targetProp.Name}]: {propType}");
            propValue = numLit.AsInt;
        }
        else if (propType == typeof(float)) {
            if(argument.Literal is not NumericLiteral numLit) throw new ArgumentResolutionException($"expected argument[{argIndex}] to be of type: {LiteralType.Numeric} but found uncastable[{targetProp.Name}]: {propType}");
            propValue = numLit.AsFloat;
        }
        else if (propType == typeof(bool)) {
            if(argument.Literal is not BooleanLiteral boolLit) throw new ArgumentResolutionException($"expected argument[{argIndex}] to be of type: {LiteralType.Boolean} but found uncastable[{targetProp.Name}]: {propType}");
            propValue = boolLit.Value;
        }
        else {
            throw new ArgumentResolutionException($"unsupported argument[{argIndex}] of unexpected type[{targetProp.Name}]: {propType}");
        }

        targetProp.SetValue(instance, propValue);
    }

    private static TDataStructure Affect(Parameters<TQueryData, TDataStructure> args) {
        var spec = new TCommandSpec();

        for(var i = 0; i < args.Length; i++){
            try{ SetProperty(spec, i, args[i]); }
            catch(ArgumentResolutionException are) { 
                throw new ArgumentResolutionException($"Command: {GetIdentifier()} " + are.Message); 
            }
        }

        return spec.Affect(args.DataSet, args.AffectAt);
    }

    public Command() : base(GetIdentifier(), GetEffectiveIn(), Affect, GetArguments()) {

    }

}

public class Command<TQueryData, TDataStructure> : ICommand<TQueryData, TDataStructure> 
    where TQueryData : QueryData
    where TDataStructure : class {
    public class Builder
    {
        public readonly string Identifier;
        private EffectiveIn _effectiveIn = EffectiveIn.None;
        private readonly List<ArgumentDeclaration> _arguments = new();
        private Func<Parameters<TQueryData, TDataStructure>, TDataStructure> _effect = (arg) => arg.DataSet;

        private Builder(string identifier) {
            Identifier = identifier;
        }

        public static Builder For(string identifier) => new Builder(identifier);

        public Builder SetRuntime(EffectiveIn effectiveIn) {
            _effectiveIn = effectiveIn;
            return this;
        }
        public Builder AddArgument<TLiteral>(string identifier) where TLiteral : Literal {
            LiteralType type;
            if (typeof(TLiteral) == typeof(BooleanLiteral)) type = LiteralType.Boolean;
            else if (typeof(TLiteral) == typeof(StringLiteral)) type = LiteralType.String;
            else if (typeof(TLiteral) == typeof(NumericLiteral)) type = LiteralType.Numeric;
            else throw new SearchExpception($"Unexpected literal type: {typeof(TLiteral).Name} when building rule");
            
            _arguments.Add(new ArgumentDeclaration(identifier, type));
            return this;
        }

        public Builder SetEffect(Func<Parameters<TQueryData, TDataStructure>, TDataStructure> effect) {
            _effect = effect;
            return this;
        }

        public Builder ResetEffect()
        {
            _effect = arg => arg.DataSet;
            return this;
        }

        public Command<TQueryData, TDataStructure> Build() {
            var argForm = new HashSet<string>();
            var argList = new List<ArgumentDeclaration>();

            foreach (var arg in _arguments) {
                if(argForm.Contains(arg.Identifier)) continue;
                
                argList.Add(arg);
                argForm.Add(arg.Identifier);
            }
            
            return new Command<TQueryData, TDataStructure>(Identifier, _effectiveIn, _effect, 
                argList.ToArray());
        }
    }

    public string Identifier { get; }
    public EffectiveIn EffectAt { get; }
    public ArgumentDeclaration[] Arguments { get; }
    public Func<Parameters<TQueryData, TDataStructure>, TDataStructure> Effect { get; }

    protected Command(string identifier, EffectiveIn effectAt, Func<Parameters<TQueryData, TDataStructure>, TDataStructure> effect, params ArgumentDeclaration[] arguments) {
        Identifier = identifier;
        EffectAt = effectAt;
        Effect = effect;
        Arguments = arguments ?? Array.Empty<ArgumentDeclaration>();
    }

    public Argument[] With(params Literal[] literals){
        var expectedArgumentCount = Arguments.Length;
        var receivedArgumentCount = literals.Length;
        if(receivedArgumentCount != expectedArgumentCount) 
            throw new ArgumentResolutionException($"Command: {Identifier} expected {expectedArgumentCount} arguments but found {receivedArgumentCount}");

        var runtimeArgs = new List<Argument>();
        for(var i = 0; i < expectedArgumentCount; i++){
            var lit = literals[i];
            var arg = Arguments[i];
            
            if(lit.Type != arg.Type) 
                throw new ArgumentResolutionException($"Command: {Identifier} expected argument[{i}] \"{arg.Identifier}\" to be of type: {arg.Type} but found: {lit.Type}");

            runtimeArgs.Add(new Argument(arg.Identifier, lit));
        }

        return runtimeArgs.ToArray();
    }
}