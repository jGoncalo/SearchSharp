namespace SearchSharp.Engine.Commands;

public abstract class CommandTemplate<TQueryData> 
    where TQueryData : QueryData {
    public abstract IQueryable<TQueryData> Affect(IQueryable<TQueryData> query, EffectiveIn at);
}
