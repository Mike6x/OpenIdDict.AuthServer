namespace Identity.Domain.Settings;

public class ApplicationConfig
{
    public string DisplayName { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public IEnumerable<string> RedirectUri { get; set; } = new List<string>();
    public IEnumerable<string> PostLogoutRedirectUri { get; set; } = new List<string>();
    public bool PKCE { get; set; } = false;
}
