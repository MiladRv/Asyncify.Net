namespace Green.CT.Asyncify.Net.AsyncRequests.Descriptors;

public interface IRequestArgumentDescriptor
{
    object?[] GetArgument();
}