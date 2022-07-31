namespace SearchSharp.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class CommandArgumentAttribute : Attribute {
    public readonly string Name;

    public CommandArgumentAttribute(string name) {
        Name = name;
    }
}