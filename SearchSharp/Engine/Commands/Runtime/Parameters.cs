namespace SearchSharp.Engine.Commands.Runtime;

public readonly struct Parameters<TQueryData> where TQueryData : QueryData {
    public readonly IQueryable<TQueryData> SourceQuery;
    private readonly IReadOnlyDictionary<string, Argument> _arguments;

    public Parameters(IQueryable<TQueryData> query, params Argument[] arguments) {
        SourceQuery = query;
        _arguments = (arguments ?? Array.Empty<Argument>()).ToDictionary(arg => arg.Identifier);
    }
    
    public bool TryGet(string identifier, out Argument arg){
        return _arguments.TryGetValue(identifier, out arg!);
    }

    public Argument this[string identifier] => _arguments[identifier];
}