namespace SearchSharp.Core.Providers;

using SearchSharp.Core.Parser;

public class MemoryProvider<TQueryData> : SearchEngine<TQueryData>.IDataProvider 
    where TQueryData : class {

    public string Name { get; }

    private IEnumerable<TQueryData>? _staticData = null;
    private Func<IEnumerable<TQueryData>>? _dynamicData = null;

    private MemoryProvider(string? name) { Name = name ?? "MemoryProvider"; }

    public static MemoryProvider<TQueryData> FromStaticData(string name, IEnumerable<TQueryData> dataSource){ 
        var prov = new MemoryProvider<TQueryData>(name);

        prov._staticData = dataSource;

        return prov;
    }
    public static MemoryProvider<TQueryData> FromDynamicData(string name, Func<IEnumerable<TQueryData>> dataSource){ 
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
        else if(_dynamicData != null) queryable = _dynamicData()?.AsQueryable() ?? throw new Exception("Dynamic data not found");
        else throw new Exception("No data source provided");

        return queryable;
    }

}