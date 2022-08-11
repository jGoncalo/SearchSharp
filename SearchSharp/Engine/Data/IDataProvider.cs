using System.Linq.Expressions;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Result;

namespace SearchSharp.Engine.Data;

public interface IProvider<TQueryData> 
    where TQueryData : QueryData {
    string Name { get; }

    IReadOnlyDictionary<string, ICommand<TQueryData>> Commands { get; }
    ISearchResult<TQueryData> ExecuteQuery(Query query, Expression<Func<TQueryData, bool>> expression);
}