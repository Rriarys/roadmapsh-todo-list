namespace ToDoList.Api.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 60; // Default, can be overridden in appsettings.json
}
