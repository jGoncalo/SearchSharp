using SearchSharp.Engine;

namespace SearchSharp.Memory;

public static class Extensions {
    public static SearchEngine<TQueryData> AddMemoryProvider<TQueryData>(this SearchEngine<TQueryData> se,
        IEnumerable<TQueryData> staticSource,
        string? providerName = null
        ) where TQueryData : class {
            se.RegisterProvider(MemoryProvider<TQueryData>.FromStaticData(staticSource, providerName));
        return se;
    }
    public static SearchEngine<TQueryData> AddMemoryProvider<TQueryData>(this SearchEngine<TQueryData> se,
        Func<IEnumerable<TQueryData>> source,
        string? providerName = null
        ) where TQueryData : class {
            se.RegisterProvider(MemoryProvider<TQueryData>.FromDynamicData(source, providerName));
        return se;
    }
}