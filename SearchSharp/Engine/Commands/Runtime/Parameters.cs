namespace SearchSharp.Engine.Commands.Runtime;

public struct Parameters<TQueryData> where TQueryData : class {
    public readonly IQueryable<TQueryData> SourceQuery;
    private readonly IReadOnlyDictionary<string, Argument> _arguments;

    public Parameters(IQueryable<TQueryData> query, params Argument[] arguments) {
        SourceQuery = query;
        _arguments = (arguments ?? Array.Empty<Argument>()).ToDictionary(arg => arg.Identifier);
    }
    
    public bool TryGet(string identifer, out Argument arg){
        return _arguments.TryGetValue(identifer, out arg!);
    }

    public Argument this[string identifer] => _arguments[identifer];
}