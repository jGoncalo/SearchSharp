using System.Linq.Expressions;
using SearchSharp.Domain;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Data;

public interface IDataProvider<TQueryData> 
    where TQueryData : QueryData {
    public string Name { get; }
    public IReadOnlyDictionary<string, ICommand<TQueryData>> Commands { get; }

    ISearchResult<TQueryData> ExecuteQuery(Query query, Expression<Func<TQueryData, bool>> expression);
}