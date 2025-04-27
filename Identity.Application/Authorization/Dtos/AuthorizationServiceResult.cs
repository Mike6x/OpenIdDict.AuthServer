namespace Identity.Application.Authorization.Dtos;

public class AuthorizationServiceResult
{
    public string? ApplicationName { get; set; } = string.Empty;
    public string? Scope { get; set; } = string.Empty;
}
