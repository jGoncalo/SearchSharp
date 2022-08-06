using SearchSharp.Engine;
using SearchSharp.Engine.Data;

namespace SearchSharp.Memory;

public static class Extensions {
    public static SearchEngine<TQueryData>.Builder AddMemoryProvider<TQueryData>(this SearchEngine<TQueryData>.Builder builder, 
        IEnumerable<TQueryData> data,
        string providerName = "MemoryProvider",
        bool isDefault = false,
        Action<DataProvider<TQueryData, MemoryRepository<TQueryData>>.Builder>? config = null) where TQueryData : QueryData {
        var repoFactory = MemoryProviderFactory<TQueryData>.FromStaticData(data);

        var providerBuilder = new DataProvider<TQueryData, MemoryRepository<TQueryData>>.Builder(providerName, repoFactory);
        if(config != null) config(providerBuilder);
        builder.RegisterProvider(providerBuilder.Build(), isDefault);

        return builder;
    }
    public static SearchEngine<TQueryData>.Builder AddMemoryProvider<TQueryData>(this SearchEngine<TQueryData>.Builder builder, 
        Func<IEnumerable<TQueryData>> source,
        string providerName = "MemoryProvider",
        bool isDefault = false,
        Action<DataProvider<TQueryData, MemoryRepository<TQueryData>>.Builder>? config = null) where TQueryData : QueryData {
        var repoFactory = MemoryProviderFactory<TQueryData>.FromDynamicData(source);

        var providerBuilder = new DataProvider<TQueryData, MemoryRepository<TQueryData>>.Builder(providerName, repoFactory);
        if(config != null) config(providerBuilder);
        builder.RegisterProvider(providerBuilder.Build(), isDefault);
        
        return builder;
    }
}