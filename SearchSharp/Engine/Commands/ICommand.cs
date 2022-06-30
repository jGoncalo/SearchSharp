using SearchSharp.Items;

namespace SearchSharp.Engine.Commands;

public interface ICommand<TQueryData> where TQueryData : class {
    string Identifier { get; }
    EffectiveIn EffectAt { get; }
    Argument[] Arguments { get; }

    Func<IQueryable<TQueryData>, IReadOnlyDictionary<string, Argument.Runtime>, IQueryable<TQueryData>> Effect { get; }

    IReadOnlyDictionary<string, Argument.Runtime> With(params Literal[] literals);
}