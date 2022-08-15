namespace SearchSharp.Engine.Parser.Components.Literals;

public abstract record Literal(string RawValue, LiteralType Type) : QueryItem;
