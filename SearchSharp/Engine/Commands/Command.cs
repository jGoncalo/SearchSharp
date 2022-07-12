using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Exceptions;

namespace SearchSharp.Engine.Commands;

public class Command<TQueryData> : ICommand<TQueryData> where TQueryData : class {
    public class Builder
    {
        public readonly string Identifier;
        private EffectiveIn _effectiveIn = EffectiveIn.None;
        private readonly List<Argument> _arguments = new();
        private Func<Parameters<TQueryData>, IQueryable<TQueryData>> _effect = arg => arg.SourceQuery;

        public Builder(string identifier) {
            Identifier = identifier;
        }

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

        public Builder SetEffect(Func<Parameters<TQueryData>, IQueryable<TQueryData>> effect) {
            _effect = effect;
            return this;
        }

        public Builder ResetEffect()
        {
            _effect = arg => arg.SourceQuery;
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
    public Func<Parameters<TQueryData>, IQueryable<TQueryData>> Effect { get; }

    private Command(string identifier, EffectiveIn effectAt, Func<Parameters<TQueryData>, IQueryable<TQueryData>> effect, params Argument[] arguments) {
        Identifier = identifier;
        EffectAt = effectAt;
        Effect = effect;
        Arguments = arguments ?? Array.Empty<Argument>();
    }

    public Runtime.Argument[] With(params Literal[] literals){
        var expectedArgumentCount = Arguments.Length;
        var receivedArgumentCount = literals?.Length ?? 0;
        if(receivedArgumentCount != expectedArgumentCount) 
            throw new ArgumentResolutionException($"Command: {Identifier} expected {expectedArgumentCount} arguments but found {receivedArgumentCount}");

        var runtimeArgs = new List<Runtime.Argument>();
        for(var i = 0; i < expectedArgumentCount; i++){
            var lit = literals![i];
            var arg = Arguments[i];
            
            if(lit.Type != arg.Type) 
                throw new ArgumentResolutionException($"Command: {Identifier} expected argument[{i}] to be of type: {arg.Type} but found: {lit.Type}");

            runtimeArgs.Add( new Runtime.Argument(arg.Identifier, lit));
        }

        return runtimeArgs.ToArray();
    }
}