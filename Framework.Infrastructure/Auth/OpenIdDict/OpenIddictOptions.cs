using System.ComponentModel.DataAnnotations;
using Framework.Core.Options;

namespace Framework.Core.Auth.OpenIdDict;

public class OpenIdDictOptions : IOptionsRoot
{
    [Required(AllowEmptyStrings = false)]
    public string? ClientId { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? ClientSecret { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? IssuerUrl { get; set; } = string.Empty;
}
