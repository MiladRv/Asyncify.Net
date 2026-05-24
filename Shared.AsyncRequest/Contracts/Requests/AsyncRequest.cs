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

    // volatile تضمین میکنه که هر thread مقدار واقعی رو از حافظه بخونه،
    // نه یه نسخه cache‌شده که ممکنه stale باشه
    private volatile int _status = (int)AsyncRequestStatus.Pending;
    private volatile object? _result;
    private Task? _executionTask;

    public AsyncRequestStatus Status => (AsyncRequestStatus)_status;

    public object? GetResult()
    {
        return Status != AsyncRequestStatus.Complete
            ? null
            : _result;
    }

    public AsyncRequestStatus GetStatus() => Status;

    public void Invoke(CancellationToken cancellationToken)
    {
        _executionTask = Task.Run(() => ExecuteInternal(cancellationToken), cancellationToken);
    }

    private void ExecuteInternal(CancellationToken cancellationToken)
    {
        try
        {
            _result = method.Invoke(constructor, arguments);
            _status = (int)AsyncRequestStatus.Complete;
        }
        catch (TimeoutException)
        {
            _status = (int)AsyncRequestStatus.Timeout;
        }
        catch (Exception)
        {
            _status = (int)AsyncRequestStatus.Failed;
        }
    }



}