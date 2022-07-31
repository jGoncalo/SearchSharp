namespace SearchSharp.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class ArgumentAttribute : Attribute {
    public readonly string Name;
    public readonly int Position;

    public ArgumentAttribute(string name) {
        Name = name;
        Position = int.MaxValue;
    }
    public ArgumentAttribute(string name, int position) : this(name) { Position = position; }
}