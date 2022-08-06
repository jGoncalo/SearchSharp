using SearchSharp.Engine.Data;

namespace SearchSharp.Engine.Commands;

public abstract class CommandTemplate<TQueryData, TDataRepository> 
    where TQueryData : QueryData
    where TDataRepository : IDataRepository<TQueryData> {
    public abstract void Affect(TDataRepository repository, EffectiveIn at);
}
