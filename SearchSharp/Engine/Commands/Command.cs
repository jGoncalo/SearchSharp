namespace SearchSharp.Engine.Commands;

public class Command<TQueryData> : ICommand<TQueryData> where TQueryData : class {
    public string Identifier { get; }
    public EffectiveIn EffectAt { get; }
    public Argument[] Arguments { get; }
    public Func<IQueryable<TQueryData>, IQueryable<TQueryData>> Effect { get; }

    private Command(string identifier, EffectiveIn effectAt, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect, params Argument[] arguments) {
        Identifier = identifier;
        EffectAt = effectAt;
        Effect = effect;
        Arguments = arguments ?? Array.Empty<Argument>();
    }

    public static Command<TQueryData> AtProvider(string identifier, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect, 
        params Argument[] arguments) {
        return new Command<TQueryData>(identifier, EffectiveIn.Provider, effect, arguments);
    }
    public static Command<TQueryData> AtQuery(string identifier, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect, 
        params Argument[] arguments) {
        return new Command<TQueryData>(identifier, EffectiveIn.Query, effect, arguments);
    }
    public static Command<TQueryData> AtAll(string identifier, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect, 
        params Argument[] arguments) {
        return new Command<TQueryData>(identifier, EffectiveIn.Provider | EffectiveIn.Query, effect, arguments);
    }
}