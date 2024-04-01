namespace Green.CT.Asyncify.Net.Contracts;

public class AsyncRequestAttribute(string methodName) : Attribute
{
    public string GetMethodName() => methodName;
}