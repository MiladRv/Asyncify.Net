using System.Reflection;

namespace Green.CT.Asyncify.Net.Contracts.Requests;

internal class AsyncRequest(
    MethodInfo method,
    object constructor,
    object[] arguments)
    : IAsyncRequest
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
    public AsyncRequestStatus Status { get; private set; } = AsyncRequestStatus.Pending;
    private object? Result { get; set; }

    //TODO:  throw exception when task not completed? 
    public object? GetResult()
    {
        return Status != AsyncRequestStatus.Complete 
            ? null 
            : Result;
    }

    public AsyncRequestStatus GetStatus() => Status;

    public void Invoke(CancellationToken cancellationToken)
    {
        try
        {
            Result = method.Invoke(constructor, arguments);
        }
        catch (TimeoutException)
        {
            Status = AsyncRequestStatus.Timeout;
        }
        catch (Exception)
        {
            Status = AsyncRequestStatus.Failed;
        }

        Status = AsyncRequestStatus.Complete;
    }



}