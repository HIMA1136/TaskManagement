namespace TaskManagement.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<T?> GetAsync<T>(Guid key, CancellationToken cancellationToken = default);
    Task StoreAsync<T>(Guid key, string requestName, T response, CancellationToken cancellationToken = default);
}
