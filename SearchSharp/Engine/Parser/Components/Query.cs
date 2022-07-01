using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public class Query : QueryItem {
    public readonly Expression Root;
    public readonly Command[] Commands;

    public Query(Expression root, params Command[] commands) {
        Root = root;
        Commands = commands ?? Array.Empty<Command>();
    }
}