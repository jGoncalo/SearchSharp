using System.Linq.Expressions;

namespace SearchSharp.Engine.Repositories;

/// <summary>
/// Data repository to be used by a given Search Provider
/// </summary>
/// <typeparam name="TQueryData">Data type</typeparam>
/// <typeparam name="TDataStructure">Class tha represents the structure containing data (ex: array, queryable, etc)</typeparam>
public interface IRepository<TQueryData, TDataStructure> 
    where TQueryData : QueryData 
    where TDataStructure : class {

    /// <summary>
    /// Instane of the DataStructure
    /// </summary>
    public TDataStructure DataSet { get; }

    /// <summary>
    /// Modify the repository to discriminate results for a given modifier
    /// </summary>
    /// <param name="modifer">function to modify the data structure</param>
    /// <param name="ct">cancellation token</param>
    /// <returns>Task</returns>
    Task ModifyAsync(Func<TDataStructure, TDataStructure> modifer, CancellationToken ct = default);
    /// <summary>
    /// Modify the repository to discriminate results for a given modifier
    /// </summary>
    /// <param name="modifer">function to modify the data structure</param>
    void Modify(Func<TDataStructure, TDataStructure> modifer);

    /// <summary>
    /// Apply a given condition to the repository
    /// </summary>
    /// <param name="condition">expression for a given constriction of results</param>
    /// <param name="ct">cancellation token</param>
    /// <returns>Task</returns>
    Task ApplyAsync(Expression<Func<TQueryData, bool>> condition, CancellationToken ct = default);
    /// <summary>
    /// Apply a given condition to the repository
    /// </summary>
    /// <param name="condition">expression for a given constriction of results</param>
    void Apply(Expression<Func<TQueryData, bool>> condition);

    /// <summary>
    /// Count records in repository matching the applyed conditions and modifiers
    /// </summary>
    /// <param name="ct">cancellation token</param>
    /// <returns>total available results</returns>
    Task<int> CountAsync(CancellationToken ct = default);
    /// <summary>
    /// Count records in repository matching the applyed conditions and modifiers
    /// </summary>
    /// <returns>total available results</returns>
    int Count();

    /// <summary>
    /// Obtain all records matching the applyed conditions and modifiers
    /// </summary>
    /// <param name="ct">cancellation token</param>
    /// <returns>Available records</returns>
    Task<TQueryData[]> FetchAsync(CancellationToken ct = default);
    /// <summary>
    /// Obtain all records matching the applyed conditions and modifiers
    /// </summary>
    /// <returns>Available records</returns>
    TQueryData[] Fetch();

}