using System.Collections;
using SearchSharp.Engine.Data.Repository;

namespace SearchSharp.Engine.Commands.Runtime;

public readonly struct Parameters<TQueryData> : IEnumerable<Argument> where TQueryData : QueryData {
    public readonly IDataRepository<TQueryData> Repository;
    public readonly EffectiveIn AffectAt;
    private readonly IReadOnlyDictionary<string, Argument> _arguments;

    public Parameters(EffectiveIn affectAt, IDataRepository<TQueryData> repository, params Argument[] arguments) {
        Repository = repository;
        AffectAt = affectAt;
        _arguments = (arguments ?? Array.Empty<Argument>()).ToDictionary(arg => arg.Identifier);
    }
    
    public bool TryGet(string identifier, out Argument arg){
        return _arguments.TryGetValue(identifier, out arg!);
    }

    public Argument this[int index] => _arguments.Values.ToArray()[index];
    public Argument this[string identifier] => _arguments[identifier];
    
    public int Length => _arguments.Values.Count();
    
    public IEnumerator<Argument> GetEnumerator() => _arguments.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}