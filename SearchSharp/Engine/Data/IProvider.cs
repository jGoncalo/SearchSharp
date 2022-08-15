using System.Linq.Expressions;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Result;

namespace SearchSharp.Engine.Data;

public interface IProvider<TQueryData> 
    where TQueryData : QueryData {
    string Name { get; }

    IReadOnlyDictionary<string, ICommand<TQueryData>> Commands { get; }
    ISearchResult<TQueryData> Get(Command[] commands, Expression<Func<TQueryData, bool>>? expression = null);
}