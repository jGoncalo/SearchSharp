using SearchSharp.Engine;

namespace SearchSharp.Memory;

public class MemoryProvider<TQueryData> : SearchEngine<TQueryData>.IDataProvider 
    where TQueryData : class {

    public string Name { get; }

    private IEnumerable<TQueryData>? _staticData = null;
    private Func<IEnumerable<TQueryData>>? _dynamicData = null;

    private MemoryProvider(string? name) { Name = name ?? "MemoryProvider"; }

    public static MemoryProvider<TQueryData> FromStaticData(IEnumerable<TQueryData> dataSource, string? name = null){ 
        var prov = new MemoryProvider<TQueryData>(name);

        prov._staticData = dataSource;

        return prov;
    }
    public static MemoryProvider<TQueryData> FromDynamicData(Func<IEnumerable<TQueryData>> dataSource, string? name = null){ 
        var prov = new MemoryProvider<TQueryData>(name);

        prov._dynamicData = dataSource;

        return prov;
    }

    public Task<IQueryable<TQueryData>> DataSourceAsync(CancellationToken ct = default) {
        return Task.FromResult(DataSource());
    }
    public IQueryable<TQueryData> DataSource() {
        IQueryable<TQueryData> queryable;

        if(_staticData != null) queryable = _staticData.AsQueryable();
        else if(_dynamicData != null) queryable = _dynamicData()?.AsQueryable() ?? throw new Exception("No data source provided");
        else throw new Exception("No data source provided");

        return queryable;
    }
}
