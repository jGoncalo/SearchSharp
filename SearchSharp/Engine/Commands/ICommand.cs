namespace SearchSharp.Engine.Commands;

public interface ICommand<TQueryData> where TQueryData : class {
    string Identifier { get; }
    EffectiveIn EffectAt { get; }

    Func<IQueryable<TQueryData>, IQueryable<TQueryData>> Effect { get; }
}