using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Application.Clients
{
    public class ApplicationDto
    {        
        public string Id { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        public string ApplicationType { get; set; } = ApplicationTypes.Web;
        public string ClientType { get; set; } = string.Empty;
        public string? ClientSecret { get; set; }

        public string? JsonWebKeySet { get; set; }
        public string ConsentType { get; set; } = string.Empty;

        public bool IsConfidentialClient => ClientType?.Equals(ClientTypes.Confidential) ?? false;

        // public List<string> Permissions { get; set; } = []

        // public List<Uri> RedirectUris { get; set; } = []
        //
        // public List<Uri> PostLogoutRedirectUris { get; set; } = []
        //
        // public List<string> Requirements { get; set; } = []
        //
        // public Dictionary<string, string> Settings { get; set; } = new(StringComparer.Ordinal)

    }
}
