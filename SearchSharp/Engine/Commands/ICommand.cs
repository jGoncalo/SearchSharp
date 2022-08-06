using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Engine.Data;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Commands;

public interface ICommand<TQueryData> where TQueryData : QueryData {
    string Identifier { get; }
    EffectiveIn EffectAt { get; }
    Argument[] Arguments { get; }

    Runtime.Argument[] With(params Literal[] literals);
}

public interface ICommand<TQueryData, TDataRepository> : ICommand<TQueryData>
    where TQueryData : QueryData
    where TDataRepository : IDataRepository<TQueryData> {

    Action<Parameters<TQueryData, TDataRepository>> Effect { get; }
}