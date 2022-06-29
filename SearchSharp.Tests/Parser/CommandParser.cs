using SearchSharp.Engine.Parser;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class CommandParser {
    [Theory]
    [InlineData("#cmd", true)]
    [InlineData("#cmd12", true)]
    [InlineData("#cmd_12", true)]
    [InlineData("#12", false)]
    [InlineData("cmd12", false)]
    [InlineData("cmd", false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    public void CanHandle_NoLit(string value, bool success){
        var result = QueryParser.Command.TryParse(value);

        Assert.Equal(success, result.WasSuccessful);
        if(success){
            Assert.Null(result.Value.Directive);
            Assert.Equal(value.Remove(0, 1), result.Value.Identifier);
        }
    }
}