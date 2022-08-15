namespace SearchSharp.Engine.Repositories;

public interface IRepositoryFactory<TQueryData, TDataRepo, TDataStructure> 
    where TQueryData : QueryData
    where TDataStructure : class
    where TDataRepo : IRepository<TQueryData, TDataStructure> {
        TDataRepo Instance();
    }