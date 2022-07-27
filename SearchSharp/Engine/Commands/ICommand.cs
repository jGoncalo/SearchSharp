using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Commands;

public interface ICommand<TQueryData> where TQueryData : QueryData {
    string Identifier { get; }
    EffectiveIn EffectAt { get; }
    Argument[] Arguments { get; }

    Func<Parameters<TQueryData>, IQueryable<TQueryData>> Effect { get; }

    Runtime.Argument[] With(params Literal[] literals);
}