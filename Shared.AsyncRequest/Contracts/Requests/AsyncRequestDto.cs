namespace Green.CT.Asyncify.Contracts.Requests;

public class AsyncRequestDto(
    Guid trackId,
    AsyncRequestStatus status,
    object? result)
{
    public Guid TrackId { get; } = trackId;
    public AsyncRequestStatus Status { get; } = status;
    public object? Result { get; } = result;
}