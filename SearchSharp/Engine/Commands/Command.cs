using SearchSharp.Items;
using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Exceptions;

namespace SearchSharp.Engine.Commands;

public class Command<TQueryData> : ICommand<TQueryData> where TQueryData : class {
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

    public static Command<TQueryData> AtProvider(string identifier, Func<Parameters<TQueryData>, IQueryable<TQueryData>> effect, 
        params Argument[] arguments) {
        return new Command<TQueryData>(identifier, EffectiveIn.Provider, effect, arguments);
    }
    public static Command<TQueryData> AtQuery(string identifier, Func<Parameters<TQueryData>, IQueryable<TQueryData>> effect, 
        params Argument[] arguments) {
        return new Command<TQueryData>(identifier, EffectiveIn.Query, effect, arguments);
    }
    public static Command<TQueryData> AtAll(string identifier, Func<Parameters<TQueryData>, IQueryable<TQueryData>> effect, 
        params Argument[] arguments) {
        return new Command<TQueryData>(identifier, EffectiveIn.Provider | EffectiveIn.Query, effect, arguments);
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