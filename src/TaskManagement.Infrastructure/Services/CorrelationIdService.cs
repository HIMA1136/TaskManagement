using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public sealed class CorrelationIdService : ICorrelationIdService
{
    private static readonly AsyncLocal<string?> _correlationId = new();

    public string? CorrelationId
    {
        get => _correlationId.Value;
        set => _correlationId.Value = value;
    }

    public string GetOrCreate() => CorrelationId ??= Guid.NewGuid().ToString();
}
