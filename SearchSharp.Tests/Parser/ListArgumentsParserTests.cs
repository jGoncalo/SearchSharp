using SearchSharp.Engine.Parser;
using Sprache;

namespace SearchSharp.Tests.Parser;

public class ListArgumentsParserTests {
    [Fact]
    public void ListArguments_Empty(){
        var emptyInput = "";
        var numOutput = QueryParser.NumericArguments.TryParse(emptyInput);
        var strOutput = QueryParser.StringArguments.TryParse(emptyInput);
        var boolOutput = QueryParser.BooleanArguments.TryParse(emptyInput);

        Assert.False(numOutput.WasSuccessful);
        Assert.False(strOutput.WasSuccessful);
        Assert.False(boolOutput.WasSuccessful);
    }

    [Fact]
    public void ListArguments_Numeric_MixedTypes(){
        var input = "1,true,\"abc\",0.4";
        var output = QueryParser.NumericArguments.TryParse(input);

        Assert.True(output.WasSuccessful);
        Assert.NotEmpty(output.Value.Literals);
        var single = Assert.Single(output.Value.Literals);
        Assert.Equal(1.AsLiteral(), single);
    }
    [Fact]
    public void ListArguments_String_MixedTypes(){
        var input = "\"abc\",1,true,0.4";
        var output = QueryParser.StringArguments.TryParse(input);

        Assert.True(output.WasSuccessful);
        Assert.NotEmpty(output.Value.Literals);
        var single = Assert.Single(output.Value.Literals);
        Assert.Equal("abc".AsLiteral(), single);
    }
    [Fact]
    public void ListArguments_Boolean_MixedTypes(){
        var input = "true,\"abc\",1,0.4";
        var output = QueryParser.BooleanArguments.TryParse(input);

        Assert.True(output.WasSuccessful);
        Assert.NotEmpty(output.Value.Literals);
        var single = Assert.Single(output.Value.Literals);
        Assert.Equal(true.AsLiteral(), single);
    }

    [Fact]
    public void ListArguments_Numeric_Single(){
        var numInput = "1";
        var numOutput = QueryParser.NumericArguments.TryParse(numInput);

        Assert.True(numOutput.WasSuccessful);
        Assert.True(numOutput.Value.IsNumericList);
        Assert.NotEmpty(numOutput.Value.Literals);
        Assert.Equal(1.AsLiteral(), numOutput.Value.Literals.Single());
    }

    [Fact]
    public void ListArguments_String_Single(){
        var strInput = "\"abc\"";
        var strOutput = QueryParser.StringArguments.TryParse(strInput);

        Assert.True(strOutput.WasSuccessful);
        Assert.True(strOutput.Value.IsStringList);
        Assert.NotEmpty(strOutput.Value.Literals);
        Assert.Equal("abc".AsLiteral(), strOutput.Value.Literals.Single());
    }

    [Fact]
    public void ListArguments_Boolean_Single(){
        var boolInput = "true";
        var boolOutput = QueryParser.BooleanArguments.TryParse(boolInput);

        Assert.True(boolOutput.WasSuccessful);
        Assert.True(boolOutput.Value.IsBooleanList);
        Assert.NotEmpty(boolOutput.Value.Literals);
        Assert.Equal(true.AsLiteral(), boolOutput.Value.Literals.Single());
    }

    [Fact]
    public void ListArguments_Numeric(){
        var numInput = "1, 2,3.4,-2,-4.4";
        var numOutput = QueryParser.NumericArguments.TryParse(numInput);

        Assert.True(numOutput.WasSuccessful);
        Assert.True(numOutput.Value.IsNumericList);
        Assert.NotEmpty(numOutput.Value.Literals);
        Assert.Equal(5, numOutput.Value.Literals.Length);

        Assert.Equal(1.AsLiteral(), numOutput.Value.Literals[0]);
        Assert.Equal(2.AsLiteral(), numOutput.Value.Literals[1]);
        Assert.Equal(3.4f.AsLiteral(), numOutput.Value.Literals[2]);
        Assert.Equal((-2).AsLiteral(), numOutput.Value.Literals[3]);
        Assert.Equal((-4.4f).AsLiteral(), numOutput.Value.Literals[4]);
    }

    [Fact]
    public void ListArguments_String(){
        var strInput = "\"abc\",\"#f30032\" ,\"efg\"";
        var strOutput = QueryParser.StringArguments.TryParse(strInput);

        Assert.True(strOutput.WasSuccessful);
        Assert.True(strOutput.Value.IsStringList);
        Assert.NotEmpty(strOutput.Value.Literals);
        Assert.Equal(3, strOutput.Value.Literals.Length);
        Assert.Equal("abc".AsLiteral(), strOutput.Value.Literals[0]);
        Assert.Equal("#f30032".AsLiteral(), strOutput.Value.Literals[1]);
        Assert.Equal("efg".AsLiteral(), strOutput.Value.Literals[2]);
    }

    [Fact]
    public void ListArguments_Boolean(){
        var boolInput = "true,FALSE,false ,True, TrUe";
        var boolOutput = QueryParser.BooleanArguments.TryParse(boolInput);

        Assert.True(boolOutput.WasSuccessful);
        Assert.True(boolOutput.Value.IsBooleanList);
        Assert.NotEmpty(boolOutput.Value.Literals);
        Assert.Equal(5, boolOutput.Value.Literals.Length);

        Assert.Equal(true.AsLiteral(), boolOutput.Value.Literals[0]);
        Assert.Equal(false.AsLiteral(), boolOutput.Value.Literals[1]);
        Assert.Equal(false.AsLiteral(), boolOutput.Value.Literals[2]);
        Assert.Equal(true.AsLiteral(), boolOutput.Value.Literals[3]);
        Assert.Equal(true.AsLiteral(), boolOutput.Value.Literals[4]);
    }

}