namespace SearchSharp.Engine.Commands;

public class Command<TQueryData> : ICommand<TQueryData> where TQueryData : class {
    public string Identifier { get; }
    public EffectiveIn EffectAt { get; }
    public Func<IQueryable<TQueryData>, IQueryable<TQueryData>> Effect { get; }

    private Command(string identifier, EffectiveIn effectAt, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect) {
        Identifier = identifier;
        EffectAt = effectAt;
        Effect = effect;
    }

    public static Command<TQueryData> AtProvider(string identifier, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect) {
        return new Command<TQueryData>(identifier, EffectiveIn.Provider, effect);
    }
    public static Command<TQueryData> AtQuery(string identifier, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect) {
        return new Command<TQueryData>(identifier, EffectiveIn.Query, effect);
    }
    public static Command<TQueryData> AtAll(string identifier, Func<IQueryable<TQueryData>, IQueryable<TQueryData>> effect) {
        return new Command<TQueryData>(identifier, EffectiveIn.Provider | EffectiveIn.Query, effect);
    }
}