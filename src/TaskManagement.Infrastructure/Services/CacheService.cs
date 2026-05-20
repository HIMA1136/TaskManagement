using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public sealed class CacheService(IMemoryCache memoryCache) : ICacheService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (!memoryCache.TryGetValue(key, out var cached))
            return Task.FromResult<T?>(default);

        if (cached is string json)
        {
            var deserialized = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            return Task.FromResult(deserialized);
        }

        return Task.FromResult(cached is T value ? value : default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);
        else
            options.SetSlidingExpiration(TimeSpan.FromMinutes(5));

        var json = JsonSerializer.Serialize(value, _jsonOptions);
        memoryCache.Set(key, json, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
