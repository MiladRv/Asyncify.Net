namespace Green.CT.Asyncify.Net.Contracts;

[AttributeUsage(AttributeTargets.Class)]
public class AsyncControllerAttribute(Type? type) : Attribute
{
    public Type? GetControllerType() => type;
}