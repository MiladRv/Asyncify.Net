using System.ComponentModel;

namespace Green.CT.Asyncify.Net.Extensions;

public static class ConverterExtensions
{
    public static object? Convert(object? value, Type type)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            return default;

        var convertedValue = TypeDescriptor
            .GetConverter(type)
            .ConvertFromInvariantString(value.ToString());

        return convertedValue;
    }

    public static object? Convert(object? value, TypeCode type)
    {
        return string.IsNullOrEmpty(value?.ToString()) 
            ? default 
            : System.Convert.ChangeType(value, type);
    }

}