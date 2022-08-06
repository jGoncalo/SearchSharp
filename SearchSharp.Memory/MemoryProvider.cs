using System.Linq.Expressions;
using SearchSharp.Engine;
using SearchSharp.Engine.Data;

namespace SearchSharp.Memory;

public class MemoryProviderFactory<TQueryData> : IDataProviderFactory<TQueryData, MemoryRepository<TQueryData>> where TQueryData : QueryData {
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

public class MemoryRepository<TQueryData> : IDataRepository<TQueryData>
    where TQueryData : QueryData {

    public delegate IQueryable<TQueryData> Modify(IQueryable<TQueryData> query);

    private IQueryable<TQueryData> _query;

    internal MemoryRepository(IQueryable<TQueryData> data) {
        _query = data;
    }

    public void Apply(Modify modifier){
        _query = modifier(_query);
    }
    public void Apply(Expression<Func<TQueryData, bool>> condition)
    {
        _query = _query.Where(condition);
    }
    public int Count() => _query.Count();
    public TQueryData[] Fetch() => _query.ToArray();
}
