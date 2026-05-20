using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Entities;

namespace TaskManagement.Infrastructure.Services;

public sealed class IdempotencyService(ApplicationDbContext context) : IIdempotencyService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<T?> GetAsync<T>(Guid key, CancellationToken cancellationToken = default)
    {
        var record = await context.IdempotencyKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.Key == key, cancellationToken);

        if (record is null) return default;

        return JsonSerializer.Deserialize<T>(record.Response, _jsonOptions);
    }

    public async Task StoreAsync<T>(Guid key, string requestName, T response, CancellationToken cancellationToken = default)
    {
        var record = new IdempotencyKey
        {
            Key = key,
            RequestName = requestName,
            Response = JsonSerializer.Serialize(response, _jsonOptions),
            CreatedAt = DateTime.UtcNow
        };

        await context.IdempotencyKeys.AddAsync(record, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
