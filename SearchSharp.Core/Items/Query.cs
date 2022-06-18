namespace SearchSharp.Core.Items;

using SearchSharp.Core.Items.Expressions;

public class Query : QueryItem {
    public readonly Expression Root;

    public Query(Expression root) {
        Root = root;
    }
}