namespace SearchSharp.Attributes;

/// <summary>
/// Specify the attributes of a Command Template
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class CommandAttribute : Attribute {
    /// <summary>
    /// Name of the command (case sensitive)
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// When will the command execute
    /// </summary>
    public readonly EffectiveIn ExecuteAt;

    /// <summary>
    /// Command attributes
    /// </summary>
    /// <param name="name">Name of the command (case sensitive)</param>
    /// <param name="executeAt">When will the command execute</param>
    public CommandAttribute(string name, EffectiveIn executeAt = EffectiveIn.Query) {
        Name = name;
        ExecuteAt = executeAt;
    }
}