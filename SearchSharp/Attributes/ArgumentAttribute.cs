using SearchSharp.Engine.Converters;

namespace SearchSharp.Attributes;
/// <summary>
/// Specify a given property settings in a commmand template
/// Warning: Attribute must have a public setter
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class ArgumentAttribute : Attribute {
    /// <summary>
    /// Name of the argument
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// Position in the argument list, if tie break using alphabetical ascending order
    /// </summary>
    public readonly int Position;
    /// <summary>
    /// Expected Literal type, when null best match will be used
    /// </summary>
    public readonly LiteralType? LitType;
    /// <summary>
    /// Converter used to convert to/from C# type to Literal type
    /// </summary>
    public readonly Type ConverterType;

    private static void AssureConverterType(Type? converterType) {
        if(converterType != null){
            if(!(converterType.IsAssignableFrom(typeof(IConverter)) || converterType.GetInterfaces().Contains(typeof(IConverter))))
                throw new ArgumentException($"Expected type to implement interface: {typeof(IConverter).FullName}", nameof(converterType));
            if(converterType.IsInterface)
                throw new ArgumentException($"Expected type to not be an interface", nameof(converterType));
            if(converterType.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"Expected type to have parameterless constructor", nameof(converterType));
        }
    }

    /// <summary>
    /// Argument specification
    /// </summary>
    /// <param name="name">Argument name</param>
    /// <param name="literalType">Literal Type of argument</param>
    /// <param name="position">Position in argument list</param>
    /// <param name="converterType">Converter type used to/from C# to Literal type</param>
    public ArgumentAttribute(string name, LiteralType literalType, int position = int.MaxValue, Type? converterType = null) {
        AssureConverterType(converterType);

        Name = name;
        Position = position;
        LitType = literalType;
        ConverterType = converterType ?? typeof(DefaultConverter);
    }
    /// <summary>
    /// Argument specification
    /// </summary>
    /// <param name="name">Argument name</param>
    /// <param name="position">Position in argument list</param>
    /// <param name="converterType">Converter type used to/from C# to Literal type</param>
    public ArgumentAttribute(string name, int position = int.MaxValue, Type? converterType = null){
        AssureConverterType(converterType);

        Name = name;
        Position = position;
        LitType = null;
        ConverterType = converterType ?? typeof(DefaultConverter);
    }

    /// <summary>
    /// Obtain instance of converter for argument
    /// </summary>
    /// <returns>C# type to/from Literal converter</returns>
    public IConverter ConverterInstance() {
        var converter = (Activator.CreateInstance(ConverterType) as IConverter)!;
        return converter;
    }
}