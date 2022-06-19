namespace SearchSharp.Items;

using SearchSharp.Items.Expressions;

public class Query : QueryItem {
    public readonly Expression Root;

    public Query(Expression root) {
        Root = root;
    }
}