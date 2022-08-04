using SearchSharp.Engine.Data.Repository;

namespace SearchSharp.Engine.Commands;

public abstract class CommandTemplate<TQueryData> 
    where TQueryData : QueryData {
    public abstract void Affect(IDataRepository<TQueryData> repo, EffectiveIn at);
}
