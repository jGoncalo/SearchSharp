using SearchSharp.Items;
using SearchSharp.Engine.Commands.Runtime;

namespace SearchSharp.Engine.Commands;

public interface ICommand<TQueryData> where TQueryData : class {
    string Identifier { get; }
    EffectiveIn EffectAt { get; }
    Argument[] Arguments { get; }

    Func<Parameters<TQueryData>, IQueryable<TQueryData>> Effect { get; }

    Runtime.Argument[] With(params Literal[] literals);
}