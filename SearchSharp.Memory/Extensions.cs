using SearchSharp.Engine;

namespace SearchSharp.Memory;

public static class Extensions {
    public static SearchEngine<TQueryData>.Builder AddMemoryProvider<TQueryData>(this SearchEngine<TQueryData>.Builder builder,
        IEnumerable<TQueryData> staticSource,
        string? providerName = null
        ) where TQueryData : QueryData {
            builder.RegisterProvider(MemoryProvider<TQueryData>.FromStaticData(staticSource, providerName));
        return builder;
    }
    public static SearchEngine<TQueryData>.Builder AddMemoryProvider<TQueryData>(this SearchEngine<TQueryData>.Builder builder,
        Func<IEnumerable<TQueryData>> source,
        string? providerName = null
        ) where TQueryData : QueryData {
            builder.RegisterProvider(MemoryProvider<TQueryData>.FromDynamicData(source, providerName));
        return builder;
    }
}