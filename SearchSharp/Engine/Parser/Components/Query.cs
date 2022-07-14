using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public class Query : QueryItem {
    public readonly string? Provider;
    public readonly Expression Root;
    public readonly Command[] Commands;

    public Query(Expression root, string? provider, params Command[] commands) {
        Root = root;
        Provider = provider;
        Commands = commands ?? Array.Empty<Command>();
    }
}