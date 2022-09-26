using System.Reflection;
using SearchSharp.Attributes;
using SearchSharp.Engine.Parser.Components.Literals;
using SearchSharp.Exceptions;
using SearchSharp.Engine.Converters;

namespace SearchSharp.Engine.Commands;

/// <summary>
/// Command specification
/// </summary>
/// <typeparam name="TQueryData">Data type associated with the command</typeparam>
/// <typeparam name="TDataStructure">Data structure associated with the command</typeparam>
public class Command<TQueryData, TDataStructure> : ICommand<TQueryData, TDataStructure> 
    where TQueryData : QueryData
    where TDataStructure : class {
    /// <summary>
    /// Command specification builder
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Unique command identifier
        /// </summary>
        public readonly string Identifier;
        private EffectiveIn _effectiveIn = EffectiveIn.None;
        private readonly List<ArgumentDeclaration> _arguments = new();
        private Func<Parameters<TQueryData, TDataStructure>, TDataStructure> _effect = (arg) => arg.DataSet;

        private Builder(string identifier) {
            Identifier = identifier;
        }

        /// <summary>
        /// Create a command builder for a given identifier
        /// </summary>
        /// <param name="identifier">Unique command identifier</param>
        /// <returns>Command Builder</returns>
        public static Builder For(string identifier) => new Builder(identifier);

        /// <summary>
        /// Set when the command will take effect
        /// </summary>
        /// <param name="effectiveIn">When the command will take effect</param>
        /// <returns>This builder</returns>
        public Builder SetRuntime(EffectiveIn effectiveIn) {
            _effectiveIn = effectiveIn;
            return this;
        }
        /// <summary>
        /// Register an argument for the command
        /// </summary>
        /// <typeparam name="TLiteral">Type of argument</typeparam>
        /// <param name="identifier">Unique argument identifier</param>
        /// <returns>This Builder</returns>
        /// <exception cref="SearchExpception">If literal type is unregistered</exception>
        public Builder AddArgument<TLiteral>(string identifier) where TLiteral : Literal {
            LiteralType type;
            if (typeof(TLiteral) == typeof(BooleanLiteral)) type = LiteralType.Boolean;
            else if (typeof(TLiteral) == typeof(StringLiteral)) type = LiteralType.String;
            else if (typeof(TLiteral) == typeof(NumericLiteral)) type = LiteralType.Numeric;
            else throw new SearchExpception($"Unexpected literal type: {typeof(TLiteral).Name} when building rule");
            
            _arguments.Add(new ArgumentDeclaration(identifier, type));
            return this;
        }

        /// <summary>
        /// Set the effect of the command
        /// </summary>
        /// <param name="effect">What effect command will have</param>
        /// <returns>This builder</returns>
        public Builder SetEffect(Func<Parameters<TQueryData, TDataStructure>, TDataStructure> effect) {
            _effect = effect;
            return this;
        }

        /// <summary>
        /// Reset the command effect
        /// (no effect is default)
        /// </summary>
        /// <returns>This builder</returns>
        public Builder ResetEffect()
        {
            _effect = arg => arg.DataSet;
            return this;
        }

        /// <summary>
        /// Build command
        /// </summary>
        /// <returns>Command Specification</returns>
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

    /// <summary>
    /// Unique command identifier
    /// </summary>
    public string Identifier { get; }
    /// <summary>
    /// When will a command be executed
    /// </summary>
    public EffectiveIn EffectAt { get; }
    /// <summary>
    /// Arguments expected for this command specification
    /// </summary>
    public ArgumentDeclaration[] Arguments { get; }
    /// <summary>
    /// What effect the command will have on a given Data structure when applied
    /// </summary>
    public Func<Parameters<TQueryData, TDataStructure>, TDataStructure> Effect { get; }

    /// <summary>
    /// Create a command specification
    /// </summary>
    /// <param name="identifier">Unique command identifier</param>
    /// <param name="effectAt">When the command will execute</param>
    /// <param name="effect">What is the command effect</param>
    /// <param name="arguments">Expected argument declaration</param>
    protected Command(string identifier, EffectiveIn effectAt, Func<Parameters<TQueryData, TDataStructure>, TDataStructure> effect, params ArgumentDeclaration[] arguments) {
        Identifier = identifier;
        EffectAt = effectAt;
        Effect = effect;
        Arguments = arguments ?? Array.Empty<ArgumentDeclaration>();
    }

    /// <summary>
    /// Create the argument list for a given command
    /// </summary>
    /// <param name="literals">Literals to attempt match on command execution</param>
    /// <returns>Arguments</returns>
    /// <exception cref="ArgumentResolutionException">If literals do not match expected argument declaration</exception>
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

/// <summary>
/// Command specification generated by a given Command Template class
/// </summary>
/// <typeparam name="TQueryData">Data type associated with the command</typeparam>
/// <typeparam name="TDataStructure">Data structure associated with the command</typeparam>
/// <typeparam name="TCommandSpec">Command Template class</typeparam>
public class Command<TQueryData, TDataStructure, TCommandSpec> : Command<TQueryData, TDataStructure>
    where TQueryData : QueryData
    where TDataStructure : class
    where TCommandSpec : CommandTemplate<TQueryData, TDataStructure>, new() {
    
    private static LiteralType ToLiteralType(PropertyInfo propInfo){
        var type = propInfo.PropertyType;
        var litType = Literal.TypeOf(type);
        if(litType != null) return litType.Value;

        var possibleVals = string.Join(",", Enum.GetValues<LiteralType>());
        throw new ArgumentResolutionException($"Could not translate {typeof(TCommandSpec).Name}.{propInfo.Name}:{type.Name} " +
            $"to one of the LiteralTypes [{possibleVals}]");
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
            .GetCustomAttributes(typeof(CommandAttribute), false)
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
                info.Attribute?.LitType ?? ToLiteralType(info.Property)
            ));
        }

        return argList.ToArray();
    }

    private static void SetProperty(TCommandSpec instance, int argIndex, Argument argument){
        var targetProp = typeof(TCommandSpec).GetProperties()
            .Where(prop => prop.GetCustomAttributes(typeof(ArgumentAttribute), false)
                                .Cast<ArgumentAttribute>()
                                .Any(attr => attr.Name == argument.Identifier)
                            || prop.Name == argument.Identifier)
            .Select(prop => { 
                var attribute = prop.GetCustomAttributes(typeof(ArgumentAttribute), false)
                    .Cast<ArgumentAttribute>().FirstOrDefault();
                return new {
                    PropertyType = prop.PropertyType,
                    Name = prop.Name,
                    Property = prop,
                    Attribute = attribute
                };
            }).FirstOrDefault();

        if(targetProp == null) return;
        
        var converter = targetProp.Attribute?.ConverterInstance() ?? new DefaultConverter();
        object propValue = converter.From(targetProp.PropertyType, argument.Literal);

        targetProp.Property.SetValue(instance, propValue);
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

    /// <summary>
    /// Create a new Command based on the specified Command Template
    /// </summary>
    public Command() : base(GetIdentifier(), GetEffectiveIn(), Affect, GetArguments()) {

    }

}