using SearchSharp.Engine.Providers;

namespace SearchSharp.Engine.Commands;

/// <summary>
/// Command template - base for all command classes
/// </summary>
/// <typeparam name="TQueryData">Type of data associated with command</typeparam>
/// <typeparam name="TDataStructure">Data structure associated with command</typeparam>
public abstract class CommandTemplate<TQueryData, TDataStructure> 
    where TQueryData : QueryData
    where TDataStructure : class {
    /// <summary>
    /// What effect does the command have on the data structure
    /// </summary>
    /// <param name="repository">Data structure for data repository</param>
    /// <param name="at">At what time this command execution is being called</param>
    /// <returns></returns>
    public abstract TDataStructure Affect(TDataStructure repository, EffectiveIn at);
}
