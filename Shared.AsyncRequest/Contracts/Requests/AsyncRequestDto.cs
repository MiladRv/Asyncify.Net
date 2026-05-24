namespace Green.CT.Asyncify.Net.Contracts.Requests;

public class AsyncRequestDto(
    Guid trackId,
    AsyncRequestStatus status,
    object? result,
    DateTime createdAt)
{
    public Guid TrackId { get; } = trackId;
    public AsyncRequestStatus Status { get; } = status;
    public object? Result { get; } = result;
    public DateTime CreatedAt { get; } = createdAt;
}