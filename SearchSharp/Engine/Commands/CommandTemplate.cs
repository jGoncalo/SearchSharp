using SearchSharp.Engine.Providers;

namespace SearchSharp.Engine.Commands;

public abstract class CommandTemplate<TQueryData, TDataStructure> 
    where TQueryData : QueryData
    where TDataStructure : class {
    public abstract TDataStructure Affect(TDataStructure repository, EffectiveIn at);
}
