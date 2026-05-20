namespace TaskManagement.Application.Common.Abstractions;

public interface ICachedQueryMarker
{
    string GetCacheKey();
    TimeSpan? GetExpiration();
}

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQueryMarker
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }

    string ICachedQueryMarker.GetCacheKey() => CacheKey;
    TimeSpan? ICachedQueryMarker.GetExpiration() => Expiration;
}
