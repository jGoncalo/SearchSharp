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
    public void CanHandle_NoArgs(string value, bool success){
        var result = QueryParser.CommandParser.TryParse(value);

        Assert.Equal(success, result.WasSuccessful);
        if(success){
            Assert.NotNull(result.Value.Arguments);
            Assert.Empty(result.Value.Arguments.Literals);
            Assert.Equal(value.Remove(0, 1), result.Value.Identifier);
        }
    }

    [Theory]
    [InlineData("#cmd", 0)]
    [InlineData("#cmd(12)", 1)]
    [InlineData("#cmd(12,22)", 2)]
    [InlineData("#cmd(12,\"some\")", 2)]
    [InlineData("#cmd(\"some\", 33)", 2)]
    [InlineData("#cmd(\"some\", \"some\")", 2)]
    [InlineData("#cmd(\"some\", True)", 2)]
    [InlineData("#cmd(\"some\", 12, True)", 3)]
    public void CanHandle_Args(string value, int argCount){
        var result = QueryParser.CommandParser.TryParse(value);

        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Value.Arguments);
        Assert.Equal(argCount, result.Value.Arguments.Literals.Length);
    }
}