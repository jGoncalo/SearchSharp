using SearchSharp.Engine.Converters;

namespace SearchSharp.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class ArgumentAttribute : Attribute {
    public readonly string Name;
    public readonly int Position;
    public readonly LiteralType? LitType;
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

    public ArgumentAttribute(string name, LiteralType literalType, int position = int.MaxValue, Type? converterType = null) {
        AssureConverterType(converterType);

        Name = name;
        Position = position;
        LitType = literalType;
        ConverterType = converterType ?? typeof(DefaultConverter);
    }
    public ArgumentAttribute(string name, int position = int.MaxValue, Type? converterType = null){
        AssureConverterType(converterType);

        Name = name;
        Position = position;
        LitType = null;
        ConverterType = converterType ?? typeof(DefaultConverter);
    }

    public IConverter ConverterInstance() {
        var converter = (Activator.CreateInstance(ConverterType) as IConverter)!;
        return converter;
    }
}