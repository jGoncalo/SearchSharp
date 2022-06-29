namespace SearchSharp.Items;

using SearchSharp.Items.Expressions;

public class Query : QueryItem {
    public readonly Expression Root;
    public readonly Command[] Commands;

    public Query(Expression root, params Command[] commands) {
        Root = root;
        Commands = commands ?? Array.Empty<Command>();
    }
}