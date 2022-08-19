using SearchSharp.Engine.Commands;
using SearchSharp.Engine.Converters;
using SearchSharp.Attributes;
using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Tests.Engine.Commands;

public class Data : QueryData {
    public enum Enum {
        Zero = 0,
        NotZero = 1
    }
    public int Id { get; set; }
}

#region Command Templates
public class NoArgumentCommand : CommandTemplate<Data, IQueryable<Data>>
{
    public override IQueryable<Data> Affect(IQueryable<Data> repository, EffectiveIn at)
    {
        throw new NotImplementedException("Expected");
    }
}
public class ArgumentCommand : CommandTemplate<Data, IQueryable<Data>>
{
    public int NumericInt { get; set; }
    public int NumericFloat { get; set; }
    public string String { get; set; } = "default";
    public bool Boolean { get; set; }
    public Data.Enum Enumerate { get; set; }

    public override IQueryable<Data> Affect(IQueryable<Data> repository, EffectiveIn at)
    {
        throw new NotImplementedException("Expected");
    }
}
public class BadCastCommand : CommandTemplate<Data, IQueryable<Data>>{
    public object Object { get; set; } = new Object();

    public override IQueryable<Data> Affect(IQueryable<Data> repository, EffectiveIn at)
    {
        throw new NotImplementedException("Expected");
    }
}
[Command("NoArgument", executeAt: EffectiveIn.Provider)]
public class AttrNoArgumentCommand : CommandTemplate<Data, IQueryable<Data>>
{
    public override IQueryable<Data> Affect(IQueryable<Data> repository, EffectiveIn at)
    {
        throw new NotImplementedException("Expected");
    }
}
[Command("Argument", executeAt: EffectiveIn.Query)]
public class AttrArgumentCommand : CommandTemplate<Data, IQueryable<Data>>
{
    [Argument("int", 1)]
    public int NumericInt { get; set; }
    [Argument("float", 2)]
    public int NumericFloat { get; set; }
    [Argument("str", 3)]
    public string String { get; set; } = "default";
    [Argument("bool", 4)]
    public bool Boolean { get; set; }
    [Argument("enum", 5)]
    public Data.Enum Enumerate { get; set; }

    public override IQueryable<Data> Affect(IQueryable<Data> repository, EffectiveIn at)
    {
        throw new NotImplementedException("Expected");
    }
}
[Command("BadCast")]
public class AttrBadCastCommand : CommandTemplate<Data, IQueryable<Data>>{
    [Argument("incorrect", 1)]
    public object Object { get; set; } = new Object();

    public override IQueryable<Data> Affect(IQueryable<Data> repository, EffectiveIn at)
    {
        throw new NotImplementedException("Expected");
    }
}
[Command("CustomConverter", EffectiveIn.Provider)]
public class CustomConverterCommand : CommandTemplate<Data, IQueryable<Data>>{
    public class Sub {
        public string Sample { get; set; } = string.Empty;
    }
    public class MyConverter : DefaultConverter {
        public override object From(Type type, StringLiteral stringLiteral)
        {
            if(type == typeof(Sub)){
                return new Sub{ Sample = stringLiteral.Value };
            }
            return base.From(type, stringLiteral);
        }
    }

    [Argument("sample", LiteralType.String, converterType: typeof(MyConverter))]
    public string Sample { get; set; } = string.Empty;
    [Argument("stub", LiteralType.String, converterType: typeof(MyConverter))]
    public Sub Stub {get; set; } = new Sub();

    public override IQueryable<Data> Affect(IQueryable<Data> repository, EffectiveIn at)
    {
        Assert.Equal("example", Sample);
        Assert.Equal("replaceMe", Stub.Sample);
        throw new NotImplementedException("Expected");
    }
}
#endregion

public class CommandTemplateTests {

    [Fact]
    public void CommandTemplate_NoArgumentTemplate() {
        var command = new Command<Data, IQueryable<Data>, NoArgumentCommand>();

        //Assert
        Assert.Equal(nameof(NoArgumentCommand), command.Identifier);
        Assert.Equal(EffectiveIn.Query, command.EffectAt);
        Assert.Empty(command.Arguments);
    }

    [Fact]
    public void CommandTemplate_Attribute_NoArgumentTemplate() {
        var command = new Command<Data, IQueryable<Data>, AttrNoArgumentCommand>();

        //Assert
        Assert.Equal("NoArgument", command.Identifier);
        Assert.Equal(EffectiveIn.Provider, command.EffectAt);
        Assert.Empty(command.Arguments);
    }

    [Fact]
    public void CommandTemplate_ArgumentsTemplate() {
        var command = new Command<Data, IQueryable<Data>, ArgumentCommand>();

        //Assert
        Assert.Equal(nameof(ArgumentCommand), command.Identifier);
        Assert.Equal(EffectiveIn.Query, command.EffectAt);
        Assert.Equal(new [] {
            new ArgumentDeclaration(nameof(ArgumentCommand.NumericInt), LiteralType.Numeric),
            new ArgumentDeclaration(nameof(ArgumentCommand.NumericFloat), LiteralType.Numeric),
            new ArgumentDeclaration(nameof(ArgumentCommand.String), LiteralType.String),
            new ArgumentDeclaration(nameof(ArgumentCommand.Boolean), LiteralType.Boolean),
            new ArgumentDeclaration(nameof(ArgumentCommand.Enumerate), LiteralType.String),
        }.OrderBy(arg => arg.Identifier).ToArray(), command.Arguments);
    }

    [Fact]
    public void CommandTemplate_Attribute_ArgumentsTemplate() {
        var command = new Command<Data, IQueryable<Data>, AttrArgumentCommand>();

        //Assert
        Assert.Equal("Argument", command.Identifier);
        Assert.Equal(EffectiveIn.Query, command.EffectAt);
        Assert.Equal(new [] {
            new ArgumentDeclaration("int", LiteralType.Numeric),
            new ArgumentDeclaration("float", LiteralType.Numeric),
            new ArgumentDeclaration("str", LiteralType.String),
            new ArgumentDeclaration("bool", LiteralType.Boolean),
            new ArgumentDeclaration("enum", LiteralType.String),
        }.ToArray(), command.Arguments);
    }

    [Fact]
    public void CommandTemplate_BadCastTemplate() {
        //Assemble
        var expMessage = $"Could not translate BadCastCommand.Object:{typeof(object).Name} " +
            $"to one of the LiteralTypes [Numeric,String,Boolean]";

        //Assert
        var exception = Assert.Throws<ArgumentResolutionException>(() => new Command<Data, IQueryable<Data>, BadCastCommand>());

        //Assert
        Assert.Equal(expMessage, exception.Message);
    }

    [Fact]
    public void CommandTemplate_Attribute_BadCastTemplate() {
        //Assemble
        var expMessage = $"Could not translate AttrBadCastCommand.Object:{typeof(object).Name} " +
            $"to one of the LiteralTypes [Numeric,String,Boolean]";

        //Assert
        var exception = Assert.Throws<ArgumentResolutionException>(() => new Command<Data, IQueryable<Data>, AttrBadCastCommand>());

        //Assert
        Assert.Equal(expMessage, exception.Message);
    }

    [Fact]
    public void CommandTemplte_CustomConverterTemplate(){
        //Assert
        var command = new Command<Data, IQueryable<Data>, CustomConverterCommand>();

        //Act
        var @params = new Parameters<Data, IQueryable<Data>>(EffectiveIn.Provider, 
            Array.Empty<Data>().AsQueryable(),
            new Argument("sample", "example".AsLiteral()),
            new Argument("stub", "replaceMe".AsLiteral()));
        var exception = Assert.Throws<NotImplementedException>(() => command.Effect.Invoke(@params));

        //Assert
        Assert.Equal("CustomConverter", command.Identifier);
        Assert.Equal(EffectiveIn.Provider, command.EffectAt);
        Assert.Equal(new [] {
            new ArgumentDeclaration("sample", LiteralType.String),
            new ArgumentDeclaration("stub", LiteralType.String)
        }.ToArray(), command.Arguments);

        Assert.Equal("Expected", exception.Message);
    }
}