namespace SearchSharp.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public class CommandArgumentAttribute : Attribute {
    public readonly string Name;

    public CommandArgumentAttribute(string name) {
        Name = name;
    }
}