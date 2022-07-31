namespace SearchSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class CommandAttribute : Attribute {
    public readonly string Name;
    public readonly EffectiveIn ExecuteAt;

    public CommandAttribute(string name, EffectiveIn executeAt = EffectiveIn.Query) {
        Name = name;
        ExecuteAt = executeAt;
    }
}