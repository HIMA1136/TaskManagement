namespace TaskManagement.Infrastructure.Settings;

public sealed class SeedDataSettings
{
    public const string SectionName = "SeedData";

    public bool Enabled { get; init; } = true;
    public string DefaultPassword { get; init; } = "Pass1234";
}
