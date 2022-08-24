namespace SearchSharp.Engine.Repositories;

/// <summary>
/// Factory for IRepository
/// </summary>
/// <typeparam name="TQueryData">Data type of repository</typeparam>
/// <typeparam name="TDataRepo">Type of the repository this factory creates</typeparam>
/// <typeparam name="TDataStructure">Data structure used by the repository (ex: array, queryable, etc...)</typeparam>
public interface IRepositoryFactory<TQueryData, TDataRepo, TDataStructure> 
    where TQueryData : QueryData
    where TDataStructure : class
    where TDataRepo : IRepository<TQueryData, TDataStructure> {
        /// <summary>
        /// Create a new instance of the repository
        /// </summary>
        /// <returns>Repository instance</returns>
        TDataRepo Instance();
    }
