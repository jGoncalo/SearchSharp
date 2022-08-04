using SearchSharp.Attributes;
using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Exceptions;

namespace SearchSharp.Engine.Commands;

public class Command<TQueryData, TCommandSpec> : Command<TQueryData>
    where TQueryData : QueryData
    where TCommandSpec : CommandTemplate<TQueryData>, new() {
    
    private static LiteralType ToLiteralType(Type type){
        if(type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return LiteralType.Numeric;
        if(type == typeof(string)) return LiteralType.String;
        if(type == typeof(bool)) return LiteralType.Boolean;
        if(type.IsEnum) return LiteralType.String;

        //TODO: add good exception
        throw new Exception("TODO");
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
    private static Argument[] GetArguments() {
        var argList = new List<Argument>();

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
            if(!(info.Property.GetSetMethod()?.IsPublic ?? false)) throw new SearchExpception("TODO");

            argList.Add(new Argument(
                info.Attribute?.Name ?? info.Property.Name,
                ToLiteralType(info.Property.PropertyType)
            ));
        }

        return argList.ToArray();
    }
    
    private static void SetProperty(TCommandSpec instance, int argIndex, Runtime.Argument argument){
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

    private static void Affect(Parameters<TQueryData> args) {
        var spec = new TCommandSpec();

        for(var i = 0; i < args.Length; i++){
            try{ SetProperty(spec, i, args[i]); }
            catch(ArgumentResolutionException are) { 
                throw new ArgumentResolutionException($"Command: {GetIdentifier()} " + are.Message); 
            }
        }

        spec.Affect(args.Repository, args.AffectAt);
    }

    public Command() : base(GetIdentifier(), GetEffectiveIn(), Affect, GetArguments()) {

    }

}

public class Command<TQueryData> : ICommand<TQueryData> where TQueryData : QueryData {
    public class Builder
    {
        public readonly string Identifier;
        private EffectiveIn _effectiveIn = EffectiveIn.None;
        private readonly List<Argument> _arguments = new();
        private Action<Parameters<TQueryData>> _effect = arg => {};

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
            
            _arguments.Add(new Argument(identifier, type));
            return this;
        }

        public Builder SetEffect(Action<Parameters<TQueryData>> effect) {
            _effect = effect;
            return this;
        }

        public Builder ResetEffect()
        {
            _effect = arg => {};
            return this;
        }

        public Command<TQueryData> Build() {
            var argForm = new HashSet<string>();
            var argList = new List<Argument>();

            foreach (var arg in _arguments) {
                if(argForm.Contains(arg.Identifier)) continue;
                
                argList.Add(arg);
                argForm.Add(arg.Identifier);
            }
            
            return new Command<TQueryData>(Identifier, _effectiveIn, _effect, 
                argList.ToArray());
        }
    }

    public string Identifier { get; }
    public EffectiveIn EffectAt { get; }
    public Argument[] Arguments { get; }
    public Action<Parameters<TQueryData>> Effect { get; }

    protected Command(string identifier, EffectiveIn effectAt, Action<Parameters<TQueryData>> effect, params Argument[] arguments) {
        Identifier = identifier;
        EffectAt = effectAt;
        Effect = effect;
        Arguments = arguments ?? Array.Empty<Argument>();
    }

    public Runtime.Argument[] With(params Literal[] literals){
        var expectedArgumentCount = Arguments.Length;
        var receivedArgumentCount = literals.Length;
        if(receivedArgumentCount != expectedArgumentCount) 
            throw new ArgumentResolutionException($"Command: {Identifier} expected {expectedArgumentCount} arguments but found {receivedArgumentCount}");

        var runtimeArgs = new List<Runtime.Argument>();
        for(var i = 0; i < expectedArgumentCount; i++){
            var lit = literals[i];
            var arg = Arguments[i];
            
            if(lit.Type != arg.Type) 
                throw new ArgumentResolutionException($"Command: {Identifier} expected argument[{i}] \"{arg.Identifier}\" to be of type: {arg.Type} but found: {lit.Type}");

            runtimeArgs.Add( new Runtime.Argument(arg.Identifier, lit));
        }

        return runtimeArgs.ToArray();
    }
}