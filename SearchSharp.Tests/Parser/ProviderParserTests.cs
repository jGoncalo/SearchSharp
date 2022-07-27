using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Parser.Components.Expressions;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class ProviderParserTests {
    [Fact]
    public void CanHandle_ProviderAndAlias(){
        var result = QueryParser.ProviderInfo.TryParse("<memory@user>");

        Assert.True(result.WasSuccessful);
        Assert.Equal("memory", result.Value.ProviderId);
        Assert.Equal("user", result.Value.EngineAlias);
    }
    [Fact]
    public void CanHandle_Provider(){
        var result = QueryParser.ProviderInfo.TryParse("<memory>");

        Assert.True(result.WasSuccessful);
        Assert.Equal("memory", result.Value.ProviderId);
        Assert.Null(result.Value.EngineAlias);
    }
    [Fact]
    public void CanHandle_Alias(){
        var result = QueryParser.ProviderInfo.TryParse("<@user>");

        Assert.True(result.WasSuccessful);
        Assert.Null(result.Value.ProviderId);
        Assert.Equal("user", result.Value.EngineAlias);
    }
}