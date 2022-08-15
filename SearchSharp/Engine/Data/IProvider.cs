using System.Linq.Expressions;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Data;

public interface IProvider<TQueryData> 
    where TQueryData : QueryData {
    string Name { get; }

    IReadOnlyDictionary<string, ICommand<TQueryData>> Commands { get; }
    Result<TQueryData> Get(Command[] commands, Expression<Func<TQueryData, bool>>? expression);
    Task<Result<TQueryData>> GetAsync(Command[] commands, Expression<Func<TQueryData, bool>>? expression, CancellationToken ct = default);
}