using SearchSharp.Engine.Commands.Runtime;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Commands;

public interface ICommand<TQueryData> where TQueryData : QueryData {
    string Identifier { get; }
    EffectiveIn EffectAt { get; }
    Argument[] Arguments { get; }

    Runtime.Argument[] With(params Literal[] literals);
}

public interface ICommand<TQueryData, TDataStructure> : ICommand<TQueryData>
    where TQueryData : QueryData
    where TDataStructure : class {

    Func<Parameters<TQueryData, TDataStructure>, TDataStructure> Effect { get; }
}