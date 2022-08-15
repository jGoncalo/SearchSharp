using SearchSharp.Engine.Repositories;

namespace SearchSharp.Memory;

public class MemoryProviderFactory<TQueryData> : IRepositoryFactory<TQueryData, MemoryRepository<TQueryData>, IQueryable<TQueryData>> where TQueryData : QueryData {
    private readonly Func<IEnumerable<TQueryData>> _getData;
    
    private MemoryProviderFactory(Func<IEnumerable<TQueryData>> dataSource) {
        _getData = dataSource;
    }

    public static MemoryProviderFactory<TQueryData> FromStaticData(IEnumerable<TQueryData> dataSource){ 
        return new MemoryProviderFactory<TQueryData>(() => dataSource);
    }
    public static MemoryProviderFactory<TQueryData> FromDynamicData(Func<IEnumerable<TQueryData>> dataSource){ 
        return new MemoryProviderFactory<TQueryData>(dataSource);
    }

    public MemoryRepository<TQueryData> Instance(){
        return new MemoryRepository<TQueryData>(_getData().AsQueryable());
    }
}
