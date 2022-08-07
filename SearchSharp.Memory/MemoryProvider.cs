using System.Linq.Expressions;
using SearchSharp.Engine;
using SearchSharp.Engine.Data;

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

public class MemoryRepository<TQueryData> : IRepository<TQueryData, IQueryable<TQueryData>>
    where TQueryData : QueryData {

    public IQueryable<TQueryData> DataSet { get; private set;}

    internal MemoryRepository(IQueryable<TQueryData> data) {
        DataSet = data;
    }

    public void Modify(Func<IQueryable<TQueryData>, IQueryable<TQueryData>> modifer) {
        DataSet = modifer(DataSet);
    }
    public void Apply(Expression<Func<TQueryData, bool>> condition)
    {
        DataSet = DataSet.Where(condition);
    }
    public int Count() => DataSet.Count();
    public TQueryData[] Fetch() => DataSet.ToArray();
}
