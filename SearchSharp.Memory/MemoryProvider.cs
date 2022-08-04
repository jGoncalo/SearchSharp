using SearchSharp.Engine;
using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Data;
using SearchSharp.Engine.Data.Repository;

namespace SearchSharp.Memory;

public class MemoryProvider<TQueryData> : DataProvider<TQueryData>
    where TQueryData : QueryData {

    public class Builder : DataProviderBuilder<Builder, TQueryData, MemoryProvider<TQueryData>>{
        public Builder(string name = "MemoryProvider") : base(name) {

        }

        public override MemoryProvider<TQueryData> Build()
        {
            return new MemoryProvider<TQueryData>(Name, Commands.Values.ToArray());
        }
    }

    private Func<IEnumerable<TQueryData>>? _dynamicData = null;

    private MemoryProvider(string? name, params ICommand<TQueryData>[] commands) : base(name ?? "MemoryProvider", commands) {

    }

    public static MemoryProvider<TQueryData> FromStaticData(IEnumerable<TQueryData> dataSource, string? name = null){ 
        var prov = new MemoryProvider<TQueryData>(name);

        prov._dynamicData = () => dataSource;

        return prov;
    }
    public static MemoryProvider<TQueryData> FromDynamicData(Func<IEnumerable<TQueryData>> dataSource, string? name = null){ 
        var prov = new MemoryProvider<TQueryData>(name);

        prov._dynamicData = dataSource;

        return prov;
    }

    protected override IDataRepository<TQueryData> GetRepository()
    {
        IQueryable<TQueryData> queryable;

        if(_dynamicData != null) queryable = _dynamicData()?.AsQueryable() ?? throw new Exception("No data source provided");
        else throw new Exception("No data source provided");

        return new QueryRepository<TQueryData>(queryable);
    }
}
