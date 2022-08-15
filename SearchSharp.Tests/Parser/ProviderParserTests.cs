using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Parser.Components.Expressions;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class ProviderParserTests {
    [Fact]
    public void CanHandle_ProviderAndAlias(){
        var result = QueryParser.ProviderInfo.TryParse("<admins@users>");

        Assert.True(result.WasSuccessful);
        Assert.Equal("admins", result.Value.ProviderId);
        Assert.Equal("users", result.Value.EngineAlias);
    }
    [Fact]
    public void CanHandle_Provider(){
        var result = QueryParser.ProviderInfo.TryParse("<@users>");

        Assert.True(result.WasSuccessful);
        Assert.Null(result.Value.ProviderId);
        Assert.Equal("users", result.Value.EngineAlias);
    }
    [Fact]
    public void CanHandle_Alias(){
        var result = QueryParser.ProviderInfo.TryParse("<admins>");

        Assert.True(result.WasSuccessful);
        Assert.Equal("admins", result.Value.ProviderId);
        Assert.Null(result.Value.EngineAlias);
    }
}