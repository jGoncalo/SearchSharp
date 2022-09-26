using System.Collections;

namespace SearchSharp.Engine.Commands;

/// <summary>
/// Command Parameters
/// </summary>
/// <typeparam name="TQueryData">Data type associated with this parameter</typeparam>
/// <typeparam name="TDataStructure">Data structure associated with this parameter</typeparam>
public readonly struct Parameters<TQueryData, TDataStructure> : IEnumerable<Argument> 
    where TQueryData : QueryData 
    where TDataStructure : class {

    /// <summary>
    /// Data structure for this parameter
    /// </summary>
    public readonly TDataStructure DataSet;
    /// <summary>
    /// When this parameter will be applied
    /// </summary>
    public readonly EffectiveIn AffectAt;
    private readonly IReadOnlyDictionary<string, Argument> _arguments;

    /// <summary>
    /// Create a new parameter for a given data set
    /// </summary>
    /// <param name="affectAt">At what execution time will the parameters will be applying to</param>
    /// <param name="dataSet">Data structure associated with this parameter</param>
    /// <param name="arguments">Arugments for this parameters</param>
    public Parameters(EffectiveIn affectAt, TDataStructure dataSet, params Argument[] arguments) {
        DataSet = dataSet;
        AffectAt = affectAt;
        _arguments = (arguments ?? Array.Empty<Argument>()).ToDictionary(arg => arg.Identifier);
    }
    
    /// <summary>
    /// Attempt to get parameter argument
    /// </summary>
    /// <param name="identifier">Argument unique identifier</param>
    /// <param name="arg">Argument (default if none)</param>
    /// <returns>True if argument found</returns>
    public bool TryGet(string identifier, out Argument arg){
        return _arguments.TryGetValue(identifier, out arg!);
    }

    /// <summary>
    /// Access argument by position
    /// </summary>
    /// <param name="index">Argument position</param>
    /// <returns>Argument</returns>
    public Argument this[int index] => _arguments.Values.ToArray()[index];
    /// <summary>
    /// Access argument by identifier 
    /// </summary>
    /// <param name="identifier">Unique argument identifier</param>
    /// <returns>Argument</returns>
    public Argument this[string identifier] => _arguments[identifier];
    
    /// <summary>
    /// Length of arguments
    /// </summary>
    public int Length => _arguments.Values.Count();
    
    /// <summary>
    /// Argument iterator
    /// </summary>
    /// <returns>Argument iterator</returns>
    public IEnumerator<Argument> GetEnumerator() => _arguments.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}