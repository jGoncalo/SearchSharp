using System.Linq.Expressions;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Providers;

/// <summary>
/// Provider of data for a given Search Engine
/// Specification of the available commands
/// </summary>
/// <typeparam name="TQueryData"></typeparam>
public interface IProvider<TQueryData> 
    where TQueryData : QueryData {
    /// <summary>
    /// Provider name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Get a command by identifier
    /// </summary>
    /// <param name="identifier">Command identifier</param>
    /// <returns>Matching command</returns>
    public ICommand<TQueryData> this[string identifier] { get; }
    /// <summary>
    /// Try to get a command by identifier
    /// </summary>
    /// <param name="identifier">Command identifier</param>
    /// <param name="command">Matching command</param>
    /// <returns>If a command matching identifier was found</returns>
    public bool TryGet(string identifier, out ICommand<TQueryData>? command);

    /// <summary>
    /// Obtain results for a given expression, applying the list of commands
    /// </summary>
    /// <param name="expression">Constraint expression for the available data</param>
    /// <param name="commands">Commands to be applyed to data</param>
    /// <returns>Results for the given expression/commands</returns>
    Result<TQueryData> Get(Expression<Func<TQueryData, bool>>? expression, params Command[] commands);
    /// <summary>
    /// Obtain results for a given expression, applying the list of commands
    /// </summary>
    /// <param name="expression">Constraint expression for the available data</param>
    /// <param name="commands">Commands to be applyed to data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Results for the given expression/commands</returns>
    Task<Result<TQueryData>> GetAsync(Expression<Func<TQueryData, bool>>? expression, Command[]? commands = null, CancellationToken ct = default);
}