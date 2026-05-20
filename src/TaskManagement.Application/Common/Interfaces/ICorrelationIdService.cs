namespace TaskManagement.Application.Common.Interfaces;

public interface ICorrelationIdService
{
    string? CorrelationId { get; set; }
    string GetOrCreate();
}
