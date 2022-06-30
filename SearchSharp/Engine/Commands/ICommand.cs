namespace SearchSharp.Engine.Commands;

public interface ICommand<TQueryData> where TQueryData : class {
    string Identifier { get; }
    EffectiveIn EffectAt { get; }
    Argument[] Arguments { get; }

    Func<IQueryable<TQueryData>, IQueryable<TQueryData>> Effect { get; }
}