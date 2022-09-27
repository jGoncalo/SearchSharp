using SearchSharp.Engine.Parser;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class NumericLiteralParserTests {
    [Theory]
    [InlineData(-1.55f)]
    [InlineData(-1.42f)]
    [InlineData(0.42f)]
    [InlineData(1.00f)]
    [InlineData(1.42f)]
    [InlineData(1.55f)]
    public void Float_Parser(float value)
    {
        var text = $"{value:0.00}";
        
        var output = QueryParser.Float.TryParse(text);

        Assert.True(output.WasSuccessful);
        Assert.Equal(value, output.Value.AsFloat);
        Assert.Equal((int) value, output.Value.AsInt);
        Assert.Equal(text, output.Value.RawValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Int_Parser(int value)
    {
        var text = $"{value}";
        
        var output = QueryParser.Int.TryParse(text);

        Assert.True(output.WasSuccessful);
        Assert.Equal(value, output.Value.AsInt);
        Assert.Equal((float) value, output.Value.AsFloat);
        Assert.Equal(text, output.Value.RawValue);
    }

    [Theory]
    [InlineData("-1.55")]
    [InlineData("-1.42")]
    [InlineData("0.42")]
    [InlineData("1.00")]
    [InlineData("1.42")]
    [InlineData("1.55")]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("1")]
    public void Numeric_Parser(string value)
    {
        var floatValue = float.Parse(value);
        var intValue = (int) floatValue;
        var output = QueryParser.Numeric.TryParse(value);

        Assert.True(output.WasSuccessful);
        Assert.Equal(intValue, output.Value.AsInt);
        Assert.Equal(floatValue, output.Value.AsFloat);
        Assert.Equal(value, output.Value.RawValue);
    }
}