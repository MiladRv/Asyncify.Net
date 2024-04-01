namespace Green.CT.Asyncify.Contracts;

public class AsyncRequestAttribute(string methodName) : Attribute
{
    public string GetMethodName() => methodName;
}