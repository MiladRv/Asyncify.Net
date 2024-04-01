namespace Green.CT.Asyncify.Net.Contracts;

[AttributeUsage(AttributeTargets.Class)]
public class AsyncControllerAttribute(Type? type) : Attribute
{
    public new Type? GetType() => type;
}